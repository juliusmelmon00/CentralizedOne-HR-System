using CentralizedOne.Data;
using CentralizedOne.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CentralizedOne.WPF.Data
{
    public class AppDbContext : ApplicationDbContext
    {
        public AppDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // You already inherit all DbSets like Users, Documents, Appointments, Notifications
    }
}
