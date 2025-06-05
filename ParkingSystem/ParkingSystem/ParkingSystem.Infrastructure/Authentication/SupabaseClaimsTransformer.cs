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
            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
                return Task.FromResult(principal);

            // Skip if role is already added
            if (identity.HasClaim(c => c.Type == ClaimTypes.Role))
                return Task.FromResult(principal);

            // Find and parse the "user_metadata" claim
            var userMetadataClaim = identity.FindFirst("user_metadata");
            if (userMetadataClaim != null)
            {
                try
                {
                    using var doc = JsonDocument.Parse(userMetadataClaim.Value);
                    if (doc.RootElement.TryGetProperty("role", out var roleElement))
                    {
                        var role = roleElement.GetString();
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("Failed to parse user_metadata claim: {Message}", ex.Message);
                }
            }

            return Task.FromResult(principal);
        }
    }

}