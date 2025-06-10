namespace ParkingSystem.Application.DTOs
{
    public class MfaStatusResponse
        {
            public List<MfaFactor> totp { get; set; } = new List<MfaFactor>();
            public bool HasEnabledFactors => totp.Any(f => f.Verified);
        }
}