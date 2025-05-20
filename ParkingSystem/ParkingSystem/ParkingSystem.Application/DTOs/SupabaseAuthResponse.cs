namespace ParkingSystem.Application.DTOs
{
    public class SupabaseAuthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public SupabaseUser user { get; set; }
    }

    public class SupabaseUser
    {
        public string id { get; set; }
        public string email { get; set; }
    }
}
