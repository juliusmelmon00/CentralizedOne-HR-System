using CentralizedOne.EmployeeApp.Models;
using CentralizedOne.EmployeeApp.Services;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CentralizedOne.EmployeeApp.Views.Auth
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _http;

        public LoginWindow()
        {
            InitializeComponent();

            // normal handler; in DEV, if you have cert issues, see comment below.
            _http = new HttpClient
            {
                BaseAddress = new Uri(SessionManager.ApiBaseUrl)
            };

            // If you have localhost HTTPS certificate issues in dev:
            // var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (m, c, ch, e) => true };
            // _http = new HttpClient(handler) { BaseAddress = new Uri(SessionManager.ApiBaseUrl) };
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LblError.Visibility = Visibility.Collapsed;
            BtnLogin.IsEnabled = false;

            try
            {
                var req = new LoginRequestDto
                {
                    Username = TxtUsername.Text.Trim(),
                    Password = TxtPassword.Password
                };

                if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                {
                    ShowError("Please enter both username and password.");
                    return;
                }

                // POST /api/Auth/login
                var response = await _http.PostAsJsonAsync("/api/Auth/login", req);

                if (!response.IsSuccessStatusCode)
                {
                    ShowError("Invalid credentials or server error.");
                    return;
                }

                // Expected response: { token, username, role }
                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<LoginResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed == null || string.IsNullOrWhiteSpace(parsed.Token))
                {
                    ShowError("Invalid login response from server.");
                    return;
                }

                // Save to session
                SessionManager.Token = parsed.Token;
                SessionManager.Username = parsed.Username;
                SessionManager.Role = parsed.Role;

                // Open MainWindow
                var main = new MainWindow();
                main.Show();

                // Close login
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError("Login failed: " + ex.Message);
            }
            finally
            {
                BtnLogin.IsEnabled = true;
            }
        }

        private void ShowError(string message)
        {
            LblError.Text = message;
            LblError.Visibility = Visibility.Visible;
        }
    }
}
