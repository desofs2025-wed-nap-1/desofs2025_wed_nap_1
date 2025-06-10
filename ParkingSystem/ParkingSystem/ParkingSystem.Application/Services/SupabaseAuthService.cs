using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ParkingSystem.Application.DTOs;
using Microsoft.Extensions.Configuration;
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

            var supabaseUrl = _config["Supabase:Url"];
            var serviceRoleKey = _config["Supabase:ServiceRoleKey"];

            var request = new HttpRequestMessage(HttpMethod.Post, $"{supabaseUrl}/auth/v1/admin/users")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
            request.Headers.Add("apikey", serviceRoleKey);

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


        public async Task<MfaEnrollResponse> EnrollMfaFactor(string accessToken)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var apiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;
            var url = $"{supabaseUrl}/auth/v1/factors";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var payload = new
            {
                factor_type = "totp",
                friendly_name = "Authenticator App"
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

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"MFA enroll error: {responseContent}");
                    throw new Exception($"MFA enroll error: {responseContent}");
                }

                var enrollResponse = JsonSerializer.Deserialize<MfaEnrollResponse>(responseContent);
                return enrollResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error enrolling MFA factor: " + ex.Message);
                throw;
            }
        }


        public async Task<SupabaseAuthResponse> VerifyAndCompleteMfaEnroll(string code, string factorId, string accessToken)
        {
            var supabaseUrl = _config["Supabase:Url"];
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

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"MFA verification error: {responseContent}");
                    throw new Exception($"MFA verification error: {responseContent}");
                }

                var authResponse = JsonSerializer.Deserialize<SupabaseAuthResponse>(responseContent);
                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error verifying MFA enrollment: " + ex.Message);
                throw;
            }
        }


        public async Task<SupabaseAuthResponse> VerifyMfa(string code, string factorId, string accessToken)
        {
            var supabaseUrl = _config["Supabase:Url"];
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

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"MFA verification error: {responseContent}");
                    throw new Exception($"MFA verification error: {responseContent}");
                }

                var authResponse = JsonSerializer.Deserialize<SupabaseAuthResponse>(responseContent);
                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error verifying MFA: " + ex.Message);
                throw;
            }
        }


        public async Task<MfaStatusResponse> GetMfaStatus(string accessToken)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var apiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;
            var url = $"{supabaseUrl}/auth/v1/factors";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error fetching MFA factors: {responseContent}");
                    throw new Exception($"Error fetching MFA factors: {responseContent}");
                }

                var mfaResponse = JsonSerializer.Deserialize<MfaStatusResponse>(responseContent);
                return mfaResponse ?? new MfaStatusResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting MFA status: " + ex.Message);
                throw;
            }
        }


        public async Task<bool> DisableMfa(string factorId, string accessToken)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var apiKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;
            var url = $"{supabaseUrl}/auth/v1/factors/{factorId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error disabling MFA: " + ex.Message);
                throw;
            }
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

            try
            {
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return false;

                var factors = JsonSerializer.Deserialize<List<MfaFactor>>(responseContent);
                return factors != null && factors.Any(f => f.Verified);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error checking MFA status: " + ex.Message);
                return false;
            }
        }
    }
}