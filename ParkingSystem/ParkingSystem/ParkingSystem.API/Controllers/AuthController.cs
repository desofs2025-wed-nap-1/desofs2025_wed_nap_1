using Microsoft.AspNetCore.Mvc;
using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Services;
using ParkingSystem.Application.Helpers;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;

        public AuthController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var result = await _authService.LoginWithSupabase(dto.Email, dto.Password);

                if (result.mfa == "mfa_required")
                {
                    return Ok(new
                    {
                        mfaRequired = true,
                        factorId = result.mfa_factor_id,
                        accessToken = result.access_token
                    });
                }

                return Ok(new
                {
                    mfaRequired = false,
                    token = result.access_token,
                    user = result.user
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("verify-mfa")]
        public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaDTO dto)
        {
            try
            {
                var result = await _authService.VerifyMfa(dto.Code, dto.FactorId, dto.AccessToken);

                return Ok(new
                {
                    token = result.access_token,
                    user = result.user.email
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // **NOVO ENDPOINT PARA ENROLL MFA (TOTP)**
        [HttpPost("mfa/enroll/{userId}")]
        public async Task<IActionResult> EnrollMfa(string userId)
        {
            try
            {
                var enrollResponse = await _authService.EnrollMfaFactor(userId);

                var qrImage = MfaHelper.GenerateQrCodeImage(enrollResponse.Totp.Uri);

                return Ok(new
                {
                    factorId = enrollResponse.Id,
                    friendlyName = enrollResponse.FriendlyName,
                    qrCode = qrImage,
                    secret = enrollResponse.Totp.Secret,
                    uri = enrollResponse.Totp.Uri
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("mfa/status/{userId}")]
        public async Task<IActionResult> CheckMfaStatus(string userId)
        {
            try
            {
                var isEnabled = await _authService.IsMfaEnabled(userId);
                return Ok(new { mfaEnabled = isEnabled });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("mfa-enabled/{userId}")]
        public async Task<IActionResult> IsMfaEnabled(string userId)
        {
            try
            {
                var enabled = await _authService.IsMfaEnabled(userId);
                return Ok(new { mfaEnabled = enabled });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("mfa/activate")]
        public async Task<IActionResult> ActivateMfa([FromBody] ActivateMfaDTO dto)
        {
            try
            {
                await _authService.ActivateMfaAsync(dto.FactorId, dto.Code, dto.AccessToken);
                return Ok(new { message = "MFA activated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
