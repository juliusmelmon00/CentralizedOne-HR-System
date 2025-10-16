using CentralizedOne.Data;
using CentralizedOne.Data.Models.DTOs; // ✅ Your DTOs namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ Needed for async DB access
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("chat")]
        [Authorize] // ✅ Any logged-in user can ask
        public async Task<IActionResult> Chat([FromBody] ChatRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Message))
                return BadRequest(new ChatResponse { Reply = "Please type something." });

            // ✅ Extract user identity from JWT
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string username = User.Identity!.Name;
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            string lower = req.Message.ToLowerInvariant();

            // ✅ Simulate async delay (temporary until OpenRouter is wired in)
            await Task.Delay(10);

            // ====================================================
            // 🔹 AI Logic — Basic Pattern Matching (Expandable)
            // ====================================================

            // --- Appointments ---
            if (lower.Contains("appointment") || lower.Contains("schedule"))
            {
                var appts = await _db.Appointments
                                     .Where(a => a.UserId == userId) // ✅ Only show THIS user's appointments
                                     .OrderByDescending(a => a.ScheduledAt)
                                     .Take(3)
                                     .Select(a => new
                                     {
                                         a.Title,
                                         a.ScheduledAt,
                                         a.Status
                                     })
                                     .ToListAsync();

                if (!appts.Any())
                    return Ok(new ChatResponse { Reply = $"{username}, you have no appointments." });

                string reply = $"Here are your upcoming appointments, {username}:\n" +
                    string.Join("\n", appts.Select(a => $"- {a.Title} on {a.ScheduledAt:g} ({a.Status})"));

                return Ok(new ChatResponse { Reply = reply });
            }

            // --- Documents ---
            if (lower.Contains("document") || lower.Contains("file"))
            {
                var docs = await _db.Documents
                                    .Where(d => d.UserId == userId) // ✅ Only show this user's docs
                                    .OrderByDescending(d => d.UploadedAt)
                                    .Take(3)
                                    .Select(d => new
                                    {
                                        d.Name,
                                        d.Status
                                    })
                                    .ToListAsync();

                if (!docs.Any())
                    return Ok(new ChatResponse { Reply = $"{username}, you have no documents uploaded." });

                string reply = $"Here are your latest documents, {username}:\n" +
                    string.Join("\n", docs.Select(d => $"- {d.Name} ({d.Status})"));

                return Ok(new ChatResponse { Reply = reply });
            }

            // --- Default response ---
            return Ok(new ChatResponse
            {
                Reply = $"I'm not sure how to answer that yet, {username}. Try asking about your *appointments* or *documents!*"
            });
        }
    }
}
