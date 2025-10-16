using CentralizedOne.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CentralizedOne.WPF.Services
{
    public static class DbService
    {
        private static IConfiguration? _config;

        public static IConfiguration Config => _config ??= new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(Config.GetConnectionString("DefaultConnection"))
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
