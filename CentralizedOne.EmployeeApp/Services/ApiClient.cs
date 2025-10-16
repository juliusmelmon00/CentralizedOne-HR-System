using CentralizedOne.EmployeeApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CentralizedOne.EmployeeApp.Services
{
    public static class ApiClient
    {
        private static readonly HttpClient _Client = new HttpClient
        {
            BaseAddress = new Uri(SessionManager.ApiBaseUrl)
        };

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // ✅ Attach Authorization Token Automatically
        private static void ApplyAuthHeader()
        {
            if (!string.IsNullOrWhiteSpace(SessionManager.Token))
            {
                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.Token);
            }
        }

        // ✅ Login Function
        public static async Task<LoginResponseDto?> LoginAsync(string username, string password)
        {
            var req = new LoginRequestDto { Username = username, Password = password };
            var response = await _Client.PostAsJsonAsync("/api/Auth/login", req);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LoginResponseDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // ✅ Fetch Employee Documents
        public static async Task<List<DocumentDto>?> GetMyDocumentsAsync()
        {
            ApplyAuthHeader();
            var response = await _Client.GetAsync("/api/Documents/my");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<DocumentDto>>();
        }

        // ✅ Fetch Employee Appointments
        public static async Task<List<AppointmentDto>?> GetMyAppointmentsAsync()
        {
            ApplyAuthHeader();
            var response = await _Client.GetAsync("/api/Appointments/my");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<AppointmentDto>>();
        }

        // ✅ Send AI Chat Request
        public static async Task<ChatResponse?> AskAIAsync(string message)
        {
            ApplyAuthHeader();
            var req = new ChatRequest { Message = message };
            var response = await _Client.PostAsJsonAsync("/api/AI/chat", req);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ChatResponse>();
        }

        // ✅ Upload Document with Expiry Date (Employee)
        public static async Task<string?> UploadDocumentAsync(string filePath, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath), "File path cannot be empty.");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("The selected file does not exist.", filePath);

            ApplyAuthHeader(); // ✅ Ensure Token is applied

            using var form = new MultipartFormDataContent();
            var fileName = Path.GetFileName(filePath);

            // ✅ Send expiryDate metadata
            form.Add(new StringContent(expiryDate.ToString("yyyy-MM-dd")), "expiryDate");

            // ✅ Prepare file stream
            await using var fileStream = File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // ✅ "file" should match WebAPI parameter: IFormFile file
            form.Add(fileContent, "file", fileName);

            var response = await _Client.PostAsync("/api/Documents/upload", form);

            if (!response.IsSuccessStatusCode)
                return null;

            // ✅ Parse JSON which looks like { "url": "...", "fileName": "...", ... }
            var jsonString = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(jsonString);
                if (doc.RootElement.TryGetProperty("url", out JsonElement urlProp))
                {
                    return urlProp.GetString(); // Returning URL only
                }
            }
            catch
            {
                // ✅ If backend just returns plain string instead of JSON
                return jsonString;
            }

            return null;
        }
    }
}
