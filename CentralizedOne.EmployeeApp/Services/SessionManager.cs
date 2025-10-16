namespace CentralizedOne.EmployeeApp.Services
{
    public static class SessionManager
    {
        public static string Token { get; set; } = string.Empty;
        public static string Username { get; set; } = "User";
        public static string Role { get; set; } = "Employee";

        // Optional: Base URL to your WebAPI (adjust port if needed)
        public static string ApiBaseUrl { get; set; } = "https://localhost:7053";
    }
}
