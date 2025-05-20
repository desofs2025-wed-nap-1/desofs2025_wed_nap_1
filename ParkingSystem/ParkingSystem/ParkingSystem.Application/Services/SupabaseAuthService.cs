using System.Text;
using System.Text.Json;
using ParkingSystem.Application.DTOs;
using Microsoft.Extensions.Configuration;


namespace ParkingSystem.Application.Services
{
    public class SupabaseAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public SupabaseAuthService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<SupabaseAuthResponse> LoginWithSupabase(string email, string password)
        {
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

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Authentication error: {responseContent}");

            var authResponse = JsonSerializer.Deserialize<SupabaseAuthResponse>(responseContent);
            return authResponse;
        }
    }
}
