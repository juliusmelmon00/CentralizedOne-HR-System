using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralizedOne.Data.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;  // e.g., "Medical Checkup"

        [Required]
        public DateTime ScheduledAt { get; set; }   // Date/time of the appointment

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";  // Pending, Approved, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Auto-set when created

        public DateTime? ReviewedAt { get; set; }  // When HR/Admin approved/rejected

        public string? RejectionReason { get; set; }  // If rejected, HR/Admin provides reason

        // Foreign Key → User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }  // Navigation property
    }
}
