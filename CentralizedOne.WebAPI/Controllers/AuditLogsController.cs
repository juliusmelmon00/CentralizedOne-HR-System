using CentralizedOne.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AuditLogsController(ApplicationDbContext db)
        {
            _db = db;
        }
        // SUPER ADMIN VIEW ALL LOGS
        [HttpGet("all")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult AllLogs()
        {
            var logs = _db.AuditLogs
                          .OrderByDescending(l => l.Timestamp)
                          .ToList();
            return Ok(logs);
        }

        //EMPLOYEE VIEW ONLY THEIR LOGS
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public IActionResult MyLogs()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "No user ID found in token" });
            int userId = int.Parse(userIdClaim.Value);
            var logs = _db.AuditLogs
                          .Where(l => l.UserId == userId)
                          .OrderByDescending(l => l.Timestamp)
                          .ToList();
            return Ok(logs);
        }
    }
}