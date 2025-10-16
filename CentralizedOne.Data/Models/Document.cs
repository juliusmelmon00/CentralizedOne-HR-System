using System;
using System.ComponentModel.DataAnnotations;

namespace CentralizedOne.Data.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        // Who uploaded the document
        [Required]
        public int UserId { get; set; }

        // Status: Pending, Approved, Rejected
        [Required]
        public string Status { get; set; } = "Pending";

        // New fields
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        //Reason for Rejection
        public string? RejectionReason { get; set; }

    }
}
