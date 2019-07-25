using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace [custom]
{
    public abstract class DbContextBase : IDbContext
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public DbContextBase(string connectionString)
        {
            SessionID = Guid.NewGuid();
            _connection = CreateConnection(connectionString);
        }

        public Guid SessionID { get; }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                if (_transaction != null)
                    _transaction.Rollback();

                _connection.Close();
                _connection = null;
            }
        }

        public async Task<int> Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await SqlMapper.ExecuteAsync(_connection, sql, param, _transaction, commandTimeout, commandType);
        }

        public async Task<IEnumerable<T>> Query<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await SqlMapper.QueryAsync<T>(_connection, sql, param, _transaction, commandTimeout: commandTimeout, commandType: commandType);
        }

        public async Task<SqlMapper.GridReader> QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await SqlMapper.QueryMultipleAsync(_connection, sql, param, _transaction, commandTimeout, commandType);
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
            _transaction = null;
        }

        private SqlConnection CreateConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
