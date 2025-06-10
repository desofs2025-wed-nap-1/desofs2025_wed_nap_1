namespace ParkingSystem.Application.DTOs
{
    public class SupabaseAuthResponse
    {
        public string access_token { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public SupabaseUser user { get; set; } = new SupabaseUser();

        public string mfa { get; set; } = string.Empty;
        public string mfa_factor_id { get; set; } = string.Empty;
    }

    public class SupabaseUser
    {
        public string id { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public List<MfaFactor> factors { get; set; } = new List<MfaFactor>();
    }
}