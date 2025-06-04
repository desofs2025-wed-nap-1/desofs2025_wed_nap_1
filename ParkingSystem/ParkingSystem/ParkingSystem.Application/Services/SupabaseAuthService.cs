using System.Text;
using System.Text.Json;
using ParkingSystem.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;


namespace ParkingSystem.Application.Services
{
    public class SupabaseAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SupabaseAuthService> _logger;

        public SupabaseAuthService(IConfiguration config, ILogger<SupabaseAuthService> logger)
        {
            _config = config;
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<SupabaseAuthResponse> LoginWithSupabase(string email, string password)
        {
            _logger.LogInformation("Attempting to login user {email}", email);

            var supabaseUrl = _config["Supabase:Url"];
            var apiKey = _config["Supabase:ApiKey"];

            var url = $"{supabaseUrl}/auth/v1/token?grant_type=password";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var payload = new
            {
                email,
                password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode || responseContent == null)
                {
                    _logger.LogError($"Error logging in user: {responseContent}");
                    throw new Exception($"Authentication error when logging in.");
                }

                var authResponse = JsonSerializer.Deserialize<SupabaseAuthResponse>(responseContent);
                if (authResponse == null)
                {
                    _logger.LogError("Error de-serializing auth response.");
                    throw new FormatException($"Authentication error when logging in.");
                }
                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception thrown when logging in user: " + ex.Message);
                throw;
            }

        }
        
        public async Task<string> CreateUserAsync(string email, string password)
        {
            var payload = new
            {
                email,
                password
            };

            var supabaseUrl = _config["Supabase:Url"];
            var apiKey = _config["Supabase:ApiKey"];

            var url = $"{supabaseUrl}/auth/v1/admin/users";

            var request = new HttpRequestMessage(HttpMethod.Post, $"{supabaseUrl}/auth/v1/admin/users")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Headers.Add("apikey", apiKey);
            
            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonDocument.Parse(json);
                if (data == null || data.RootElement.GetProperty("id").GetString().IsNullOrEmpty())
                {
                    var errorMessage = "Error from Supabase - user creation response is null";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }
                return data.RootElement.GetProperty("id").GetString();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when creating user in Supabase: " + ex.Message);
                throw;
            }
            
        }
    }
    
}
