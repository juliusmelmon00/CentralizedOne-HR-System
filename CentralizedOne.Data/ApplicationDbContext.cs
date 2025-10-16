using CentralizedOne.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CentralizedOne.Data
{
    
    public class ApplicationDbContext : DbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //seed user

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "superadmin",
                    PasswordHash = User.HashPassword("admin123"),           //Super Admin
                    Role = "SuperAdmin"
                },
                new User
                {
                    Id = 2,
                    Username = "employee1",
                    PasswordHash = User.HashPassword("password123"),        //Employee
                    Role = "Employee"
                },
                 new User
                 {
                     Id = 3,
                     Username = "hradmin",
                     PasswordHash = User.HashPassword("hr123"),             //HR/Admin
                     Role = "HR/Admin"
                 }
            );
            modelBuilder.Entity<Notification>().HasData(
                new Notification
                {
                    Id = 1,
                    UserId = 2, // employee 1
                    Message = "Welcome! Your account has been created successfully.",
                    IsRead = false,
                    CreatedAt = new DateTime(2025, 09, 15, 12, 00, 00, DateTimeKind.Utc) // fixed timestamp
                },
                new Notification
                {
                    Id = 2,
                    UserId = 2, //Employee 1
                    Message = "Your documents 'Medical Ceretificate' was rejected. Reason : Missing Signature.",
                    IsRead = false,
                    CreatedAt = new DateTime(2025, 09, 15, 12, 00, 00, DateTimeKind.Utc)// fixed timestamp

                });
        }

    }
}
