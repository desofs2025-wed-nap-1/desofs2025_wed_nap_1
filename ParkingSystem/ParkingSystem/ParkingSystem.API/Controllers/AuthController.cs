using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingSystem.Application.Interfaces;
using ParkingSystem.Application.DTOs;
using ParkingSystem.Application.Services;
using System.Security.Claims;

namespace ParkingSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(SupabaseAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var result = await _authService.LoginWithSupabase(dto.Email, dto.Password);

                bool hasMfaEnabled = result.user.factors.Any(f => f.Verified);

                if (hasMfaEnabled)
                {
                    var enabledFactor = result.user.factors.First(f => f.Verified);
                    return Ok(new
                    {
                        mfaRequired = true,
                        factorId = enabledFactor.Id,
                        accessToken = result.access_token,
                        message = "MFA verification required"
                    });
                }

                return Ok(new
                {
                    mfaRequired = false,
                    token = result.access_token,
                    user = new
                    {
                        id = result.user.id,
                        email = result.user.email
                    },
                    mfaEnabled = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", dto.Email);
                return Unauthorized(new { message = "Invalid credentials" });
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
                    user = new
                    {
                        id = result.user.id,
                        email = result.user.email
                    },
                    message = "MFA verification successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MFA verification");
                return Unauthorized(new { message = "Invalid MFA code" });
            }
        }

        [HttpPost("mfa/setup")]
        [Authorize]
        public async Task<IActionResult> SetupMfa()
        {
            try
            {
                var accessToken = GetAccessTokenFromHeader();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized(new { message = "Access token required" });
                }

                var enrollResponse = await _authService.EnrollMfaFactor(accessToken);

                return Ok(new
                {
                    factorId = enrollResponse.id,
                    friendlyName = enrollResponse.friendly_name,
                    qrCode = enrollResponse.totp.qr_code,
                    secret = enrollResponse.totp.secret,
                    uri = enrollResponse.totp.uri,
                    message = "Scan the QR code with your authenticator app and enter the code to complete setup"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up MFA");
                return BadRequest(new { message = "Error setting up MFA: " + ex.Message });
            }
        }

        [HttpPost("mfa/complete-setup")]
        [Authorize]
        public async Task<IActionResult> CompleteMfaSetup([FromBody] SetupMfaDTO dto)
        {
            try
            {
                var accessToken = GetAccessTokenFromHeader();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized(new { message = "Access token required" });
                }

                var result = await _authService.VerifyAndCompleteMfaEnroll(dto.Code, dto.FactorId, accessToken);

                return Ok(new
                {
                    message = "MFA setup completed successfully",
                    token = result.access_token,
                    user = new
                    {
                        id = result.user.id,
                        email = result.user.email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing MFA setup");
                return BadRequest(new { message = "Invalid verification code" });
            }
        }

        [HttpGet("mfa/status")]
        [Authorize]
        public async Task<IActionResult> GetMfaStatus()
        {
            try
            {
                var accessToken = GetAccessTokenFromHeader();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized(new { message = "Access token required" });
                }

                var mfaStatus = await _authService.GetMfaStatus(accessToken);

                return Ok(new
                {
                    mfaEnabled = mfaStatus.HasEnabledFactors,
                    factors = mfaStatus.totp.Select(f => new
                    {
                        id = f.id,
                        friendlyName = f.friendly_name,
                        verified = f.Verified,
                        status = f.status
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting MFA status");
                return BadRequest(new { message = "Error getting MFA status" });
            }
        }

        [HttpDelete("mfa/disable/{factorId}")]
        [Authorize]
        public async Task<IActionResult> DisableMfa(string factorId)
        {
            try
            {
                var accessToken = GetAccessTokenFromHeader();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized(new { message = "Access token required" });
                }

                var success = await _authService.DisableMfa(factorId, accessToken);

                if (success)
                {
                    return Ok(new { message = "MFA disabled successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to disable MFA" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling MFA");
                return BadRequest(new { message = "Error disabling MFA" });
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
                _logger.LogError(ex, "Error checking MFA status for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
        }

        private string GetAccessTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length);
            }
            return string.Empty;
        }
    }
}