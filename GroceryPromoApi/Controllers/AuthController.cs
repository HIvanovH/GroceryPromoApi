using GroceryPromoApi.Application.DTOs.Auth;
using GroceryPromoApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;

namespace GroceryPromoApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var sessionId = Guid.Parse(User.FindFirstValue("sessionId")!);
        await _authService.LogoutAsync(sessionId, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPut("fcm-token")]
    public async Task<IActionResult> UpdateFcmToken([FromBody] string fcmToken, CancellationToken cancellationToken)
    {
        var sessionId = Guid.Parse(User.FindFirstValue("sessionId")!);
        await _authService.UpdateFcmTokenAsync(sessionId, fcmToken, cancellationToken);
        return NoContent();
    }
}
