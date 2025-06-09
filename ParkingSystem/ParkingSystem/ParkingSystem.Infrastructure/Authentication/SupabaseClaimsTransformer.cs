using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace ParkingSystem.Infrastructure.Authentication
{
    public class SupabaseClaimsTransformer : IClaimsTransformation
    {
        private readonly ILogger<SupabaseClaimsTransformer> _logger;

        public SupabaseClaimsTransformer(ILogger<SupabaseClaimsTransformer> logger)
        {
            _logger = logger;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            _logger.LogWarning("Running SupabaseClaimsTransformer");

            if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
                return Task.FromResult(principal);

            // Skip if already has a Role claim (but only if it's your custom role)
            if (identity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value != "authenticated"))
            {
                _logger.LogInformation("Custom Role already added, skipping transformation.");
                return Task.FromResult(principal);
            }

            var appMetadataClaim = identity.FindFirst("user_metadata");
            if (appMetadataClaim != null)
            {
                _logger.LogInformation("Found user_metadata, attempting to parse.");

                try
                {
                    using var doc = JsonDocument.Parse(appMetadataClaim.Value);
                    _logger.LogInformation("Parsed document: " + doc.RootElement.ToString());
                    if (doc.RootElement.TryGetProperty("role", out var roleElement))
                    {
                        var role = roleElement.GetString();
                        _logger.LogInformation("Parsed role from user_metadata: {Role}", role);

                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No 'role' field in user_metadata.");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("Failed to parse user_metadata JSON: {Message}", ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("user_metadata claim not found.");
            }

            return Task.FromResult(principal);
        }
    }


}