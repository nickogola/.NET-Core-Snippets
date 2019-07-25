namespace [custom]
{
    public class AppDbContext : DbContextBase, IAppDbContext
    {
        public AppDbContext(string connectionString) : base(connectionString)
        { }
    }
}
