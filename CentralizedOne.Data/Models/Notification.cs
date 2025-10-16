using System;

namespace CentralizedOne.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }             // Primary key
        public int UserId { get; set; }         // Which employee gets this notification
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Navigation property (optional)
        public User? User { get; set; }
    }
}
