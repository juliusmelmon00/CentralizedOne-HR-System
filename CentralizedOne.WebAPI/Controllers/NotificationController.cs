using CentralizedOne.Data;
using CentralizedOne.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public NotificationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ================================
        // EMPLOYEE: View my notifications
        // ================================
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public IActionResult MyNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "No user ID found in token" });

            int userId = int.Parse(userIdClaim.Value);

            var notifications = _db.Notifications
                                   .Where(n => n.UserId == userId)
                                   .OrderByDescending(n => n.CreatedAt)
                                   .ToList();

            return Ok(notifications);
        }

        // ================================
        // EMPLOYEE: Mark notification as read
        // ================================
        [HttpPut("mark-read/{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult MarkAsRead(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "No user ID found in token" });

            int userId = int.Parse(userIdClaim.Value);

            var notification = _db.Notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
            if (notification == null)
                return NotFound(new { message = "Notification not found" });

            notification.IsRead = true;
            _db.SaveChanges();

            return Ok(new { message = "✅ Notification marked as read", notification });
        }

        // ================================
        // HR/Admin or SuperAdmin: Send notification manually
        // ================================
        [HttpPost("send")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult Send([FromBody] Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            _db.Notifications.Add(notification);
            _db.SaveChanges();

            return Ok(new { message = "✅ Notification sent", notification });
        }



        // ================================
        // SUPERADMIN: View all notifications (optional)
        // ================================
        [HttpGet("all")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult AllNotifications()
        {
            var all = _db.Notifications.OrderByDescending(n => n.CreatedAt).ToList();
            return Ok(all);
        }
    }
}
