using GameBackend.DTOs.Auth;
using GameBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);

            return Ok(response);
        } 
        catch (InvalidOperationException ex)
        {
            return BadRequest(new {message = ex.Message});    
        }
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmailAsync(
        [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        var isVerified = await _authService.VerifyEmailAsync(token, cancellationToken);

        if (!isVerified){
            return BadRequest(new
            {
                message = "The verification link is invalid or has expired"
            });
        } 
        return Ok(new
        {
            message = "Email successfully verified!"
        });       
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(
        [FromBody] ResendVerificationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.ResendVerificationAsync(
            request.Email,
            cancellationToken);

        return Ok(response);
    }
}