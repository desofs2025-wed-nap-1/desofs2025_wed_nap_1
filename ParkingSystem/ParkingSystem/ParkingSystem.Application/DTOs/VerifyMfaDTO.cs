namespace ParkingSystem.Application.DTOs
{
    public class VerifyMfaDTO
        {
            public string Code { get; set; } = string.Empty;
            public string FactorId { get; set; } = string.Empty;
            public string AccessToken { get; set; } = string.Empty;
        }
}
