using System;

namespace CentralizedOne.Data.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        // Who performed the action (UserId)
        public int UserId { get; set; }

        // Action type (e.g., "UPLOAD", "APPROVE", "REJECT")
        public string Action { get; set; } = string.Empty;

        // Target entity (DocumentId)
        public int? DocumentId { get; set; }

        // Extra details (like rejection reason)
        public string? Details { get; set; }

        // Timestamp
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
