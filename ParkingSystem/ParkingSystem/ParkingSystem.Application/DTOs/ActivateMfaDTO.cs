namespace ParkingSystem.Application.DTOs
{
    public class ActivateMfaDTO
    {
        public string FactorId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
