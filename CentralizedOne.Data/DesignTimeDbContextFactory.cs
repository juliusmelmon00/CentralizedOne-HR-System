using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CentralizedOne.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Replace or use the same connection string used in appsettings.json
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CentralizedOne;Trusted_Connection=True;TrustServerCertificate=True;");
            return new ApplicationDbContext(builder.Options);
        }
    }
}
