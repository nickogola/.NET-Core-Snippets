namespace [custom]
{
    /*
        Inherits from DbContextBase and IAppDbContext
    */
    public class AppDbContext : DbContextBase, IAppDbContext
    {
        public AppDbContext(string connectionString) : base(connectionString)
        { }
    }
}
