using System.Net.Http.Headers;
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

            if (authResponse.mfa == "mfa_required")
            {
                // returns authResponse only when mfa and mfa_factor_id
                return authResponse;
            }

            return authResponse;
        }

        public async Task<SupabaseAuthResponse> VerifyMfa(string code, string factorId, string accessToken)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var apiKey = _config["Supabase:ApiKey"];

            var url = $"{supabaseUrl}/auth/v1/verify";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var payload = new
            {
                type = "totp",
                factor_id = factorId,
                code = code
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"MFA verification error: {responseContent}");

            var authResponse = JsonSerializer.Deserialize<SupabaseAuthResponse>(responseContent);
            return authResponse;
        }

        public async Task<MfaEnrollResponse> EnrollMfaFactor(string userId)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var serviceRoleKey = _config["Supabase:ServiceRoleKey"];
            var url = $"{supabaseUrl}/auth/v1/admin/users/{userId}/factors";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var payload = new
            {
                factor_type = "totp",
                friendly_name = "MFA"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"MFA enroll error: {responseContent}");

            return JsonSerializer.Deserialize<MfaEnrollResponse>(responseContent);
        }

        public async Task<bool> IsMfaEnabled(string userId)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var serviceRoleKey = _config["Supabase:ServiceRoleKey"];
            var url = $"{supabaseUrl}/auth/v1/admin/users/{userId}/factors";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error fetching MFA factors: {responseContent}");

            var factors = JsonSerializer.Deserialize<List<MfaFactor>>(responseContent);

            return factors != null && factors.Count > 0;
        }
    }
}
