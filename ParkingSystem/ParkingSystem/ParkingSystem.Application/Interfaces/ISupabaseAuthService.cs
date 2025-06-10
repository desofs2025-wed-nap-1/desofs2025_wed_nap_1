using ParkingSystem.Application.DTOs;

namespace ParkingSystem.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<SupabaseAuthResponse> LoginWithSupabase(string email, string passwordInput);
        Task<string> CreateUserAsync(string email, string password, string role);
        Task<SupabaseAuthResponse> VerifyMfa(string code, string factorId, string accessToken);
        Task<MfaEnrollResponse> EnrollMfaFactor(string userId);
        Task<bool> IsMfaEnabled(string userId);
        
    }
}
