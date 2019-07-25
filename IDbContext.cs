using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace [custom]
{
    public interface IDbContext : IDisposable
    {
        Guid SessionID { get; }
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        Task<int> Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<T>> Query<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<SqlMapper.GridReader> QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
        void RollbackTransaction();
    }
}
