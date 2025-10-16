using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CentralizedOne.WebAPI.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _model;

        public OpenRouterService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _apiKey = _config["AI:ApiKey"] ?? throw new ArgumentNullException("AI:ApiKey not set");
            _baseUrl = _config["AI:BaseUrl"] ?? "https://openrouter.ai/api/v1/chat/completions";
            _model = _config["AI:Model"] ?? "openai/gpt-3.5-turbo";

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _http.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<string> GetChatReplyAsync(string systemPrompt, string userMessage)
        {
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 512,
                temperature = 0.3
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync(_baseUrl, content);
            var respText = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                // return raw error so controller can decide what to do
                throw new Exception($"OpenRouter call failed: {(int)resp.StatusCode} {resp.ReasonPhrase} - {respText}");
            }

            // Try to parse response safely
            try
            {
                using var doc = JsonDocument.Parse(respText);
                // OpenRouter returns similar shape to OpenAI: choices[0].message.content
                if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var message = choices[0].GetProperty("message").GetProperty("content").GetString();
                    return message ?? string.Empty;
                }

                // fallback: return raw body
                return respText;
            }
            catch
            {
                return respText;
            }
        }
    }
}
