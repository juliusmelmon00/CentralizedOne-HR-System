using CentralizedOne.Data;
using CentralizedOne.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DocumentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ================================
        // EMPLOYEE: Upload a document
        // ================================
        [HttpPost("upload")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] DateTime expiryDate)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file selected." });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // ✅ Create user-specific folder
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", userId.ToString());
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // ✅ Generate final file path
            var filePath = Path.Combine(uploadPath, file.FileName);

            // ✅ Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ✅ Save metadata to database
            var document = new Document
            {
                Name = file.FileName,
                Status = "Pending",
                UserId = userId,
                UploadedAt = DateTime.UtcNow,
                ExpiryDate = expiryDate,
                RejectionReason = null
            };

            _db.Documents.Add(document);
            await _db.SaveChangesAsync();

            return Ok(new { message = "✅ File uploaded and pending review", document });
        }


        // ================================
        // EMPLOYEE: View own documents
        // ================================
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public IActionResult MyDocuments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "No user ID found in token" });

            int userId = int.Parse(userIdClaim.Value);
            var docs = _db.Documents
                          .Where(d => d.UserId == userId)
                          .Select(d => new
                          {
                              d.Id,
                              d.Name,
                              d.Status,
                              d.UploadedAt,
                              d.ReviewedAt,
                              d.ExpiryDate,
                              d.RejectionReason
                          })
                          .ToList();

            return Ok(docs);
        }

        // ================================
        // HR/ADMIN: See pending documents
        // ================================
        [HttpGet("pending")]
        [Authorize(Roles = "HR/Admin")]
        public IActionResult Pending()
        {
            var pendingDocs = _db.Documents
                                 .Where(d => d.Status == "Pending")
                                 .ToList();
            return Ok(pendingDocs);
        }

        // ================================
        // HR/ADMIN: Approve/Reject document + Auto Notifications
        // ================================
        [HttpPut("review/{id}")]
        [Authorize(Roles = "HR/Admin")]
        public IActionResult Review(int id, [FromQuery] bool approved, [FromQuery] string? reason = null)
        {
            var doc = _db.Documents.FirstOrDefault(d => d.Id == id);
            if (doc == null) return NotFound(new { message = "Document not found" });

            doc.Status = approved ? "Approved" : "Rejected";
            doc.ReviewedAt = DateTime.UtcNow;

            if (!approved)
            {
                doc.RejectionReason = string.IsNullOrWhiteSpace(reason)
                    ? "No reason provided"
                    : reason;

                // 🔔 Auto Notification for Rejection
                _db.Notifications.Add(new Notification
                {
                    UserId = doc.UserId,
                    Message = $"❌ Your document '{doc.Name}' was rejected. Reason: {doc.RejectionReason}",
                    CreatedAt = DateTime.UtcNow,
                   // IsRead = false // eto yung bagong lagay 
                });
            }
            else
            {
                doc.RejectionReason = null;

                // 🔔 Auto Notification for Approval
                _db.Notifications.Add(new Notification
                {
                    UserId = doc.UserId,
                    Message = $"✅ Your document '{doc.Name}' was approved!",
                    CreatedAt = DateTime.UtcNow,
                   // IsRead = false // eto yung bagong lagayyy
                });
            }

            _db.SaveChanges();

            return Ok(new { message = $"✅ Document {id} marked as {doc.Status}", doc });
        }

        // ================================
        // SUPERADMIN: See all documents
        // ================================
        [HttpGet("all")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult AllDocuments()
        {
            var allDocs = _db.Documents.ToList();
            return Ok(allDocs);
        }
    }
}
