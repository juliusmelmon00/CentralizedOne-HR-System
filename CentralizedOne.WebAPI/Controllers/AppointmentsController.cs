using CentralizedOne.Data;
using CentralizedOne.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ================================
        // EMPLOYEE: Schedule a new appointment
        // ================================
        [HttpPost("schedule")]
        [Authorize(Roles = "Employee")]
        public IActionResult Schedule([FromBody] Appointment appointment)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "No user ID in token" });

            appointment.UserId = int.Parse(userIdClaim.Value);
            appointment.Status = "Pending";
            appointment.CreatedAt = DateTime.UtcNow;

            _db.Appointments.Add(appointment);
            _db.SaveChanges();

            // ✅ Notify HR/Admin that a new appointment needs review
            var notifyHr = new Notification
            {
                UserId = 3, // TEMP: Hardcode HR/Admin ID — better: query by role
                Message = $"🕒 New appointment request from user {appointment.UserId} on {appointment.ScheduledAt:g}.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.Notifications.Add(notifyHr);
            _db.SaveChanges();

            return Ok(new
            {
                message = "✅ Appointment scheduled",
                appointment,
                notification = notifyHr
            });
        }

        // ================================
        // EMPLOYEE: View my appointments
        // ================================
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public IActionResult MyAppointments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "No user ID in token" });

            int userId = int.Parse(userIdClaim.Value);

            var appointments = _db.Appointments
                                  .Where(a => a.UserId == userId)
                                  .OrderByDescending(a => a.ScheduledAt)
                                  .ToList();

            return Ok(appointments);
        }

        // ================================
        // HR/ADMIN: View pending appointments
        // ================================
        [HttpGet("pending")]
        [Authorize(Roles = "HR/Admin")]
        public IActionResult Pending()
        {
            var pending = _db.Appointments
                             .Where(a => a.Status == "Pending")
                             .ToList();

            return Ok(pending);
        }

        // ================================
        // HR/ADMIN: Approve or reject (with notification)
        // ================================
        [HttpPut("review/{id}")]
        [Authorize(Roles = "HR/Admin")]
        public IActionResult Review(int id, [FromQuery] bool approved, [FromQuery] string? reason = null)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
                return NotFound(new { message = "Appointment not found" });

            appointment.Status = approved ? "Approved" : "Rejected";
            appointment.ReviewedAt = DateTime.UtcNow;

            if (!approved)
            {
                appointment.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : reason;
            }
            else
            {
                appointment.RejectionReason = null;
            }

            // 🔔 Auto-create notification for employee
            var notification = new Notification
            {
                UserId = appointment.UserId,
                Message = approved
                    ? $"✅ Your appointment on {appointment.ScheduledAt:g} has been approved."
                    : $"❌ Your appointment on {appointment.ScheduledAt:g} was rejected. Reason: {appointment.RejectionReason}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.Notifications.Add(notification);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Appointment {id} marked as {appointment.Status}",
                appointment,
                notification
            });
        }

        // ================================
        // SUPERADMIN: View all appointments
        // ================================
        [HttpGet("all")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult AllAppointments()
        {
            var all = _db.Appointments
                         .OrderByDescending(a => a.ScheduledAt)
                         .ToList();

            return Ok(all);
        }
    }
}
