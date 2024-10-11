using Microsoft.EntityFrameworkCore;
namespace ArmyGrievances.Repository
{
    public class AD_DBContext : DbContext
    {
        private readonly string conString;
        public AD_DBContext(string connectionString) : base()
        {
            conString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

            var connectionString = configuration.GetConnectionString("DB_Connection");
            optionsBuilder.UseSqlServer(connectionString);
        }
        public AD_DBContext(DbContextOptions<AD_DBContext> options) : base(options)
        {
        }
    }
}
