namespace CentralizedOne.EmployeeApp.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;
        public string? Raw { get; set; } // Optional, can be used later for structured data
    }
}
