using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ParkingSystem.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ParkingSystem.Application.Helpers;

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

            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var apiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;



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
                if (authResponse.mfa == "mfa_required")
                {
                    // returns authResponse only when mfa and mfa_factor_id
                    return authResponse;
                }
                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception thrown when logging in user: " + ex.Message);
                throw;
            }

        }
        
        public async Task<string> CreateUserAsync(string email, string password, string role)
        {
            var payload = new
            {
                email,
                password,
                user_metadata = new
                    {
                        role = role
                    }
            };

            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var apiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;


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

        public async Task<SupabaseAuthResponse> VerifyMfa(string code, string factorId, string accessToken)
        {
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var apiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;

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
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var serviceRoleKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY")!;
            var url = $"{supabaseUrl}/auth/v1/admin/users/{userId}/factors";

            var payload = new
            {
                factor_type = "totp",
                friendly_name = "MFA"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("apikey", serviceRoleKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"MFA enroll error: {responseContent}");

            return JsonSerializer.Deserialize<MfaEnrollResponse>(responseContent);
        }


        public async Task<bool> IsMfaEnabled(string userId)
        {
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var serviceRoleKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY")!;
            var url = $"{supabaseUrl}/auth/v1/admin/users/{userId}/factors";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("apikey", serviceRoleKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error fetching MFA factors: {responseContent}");

            var factors = JsonSerializer.Deserialize<List<MfaFactor>>(responseContent);

            return factors != null && factors.Count > 0;
        }

        public async Task ActivateMfaAsync(string factorId, string code, string accessToken)
        {
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var apiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;
            var url = $"{supabaseUrl}/auth/v1/factors/{factorId}/verify";

            var payload = new
            {
                code = code
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error activating MFA: {responseContent}");
        }

    }
    
}
