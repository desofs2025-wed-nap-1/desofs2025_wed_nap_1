using ParkingSystem.Application.DTOs;

namespace ParkingSystem.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<SupabaseAuthResponse> LoginWithSupabase(string email, string passwordInput);
        Task<string> CreateUserAsync(string email, string password, string role);
        
    }
}
