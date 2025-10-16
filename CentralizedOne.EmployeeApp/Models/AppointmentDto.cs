namespace CentralizedOne.EmployeeApp.Models
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
    }
}
