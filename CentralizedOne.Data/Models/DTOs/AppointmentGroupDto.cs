namespace CentralizedOne.Data.Models.DTOs
{
    public class AppointmentGroupDto
    {
        public string Period { get; set; } = string.Empty; // e.g., "2025-09" or "2025"-09-16"
        public int TotalAppointments { get; set; }
        public int Pending {  get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
}
