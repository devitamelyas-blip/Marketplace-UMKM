using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UmkmJatim.Application.Auth.Commands;
using UmkmJatim.Application.Auth.DTOs;

namespace UmkmJatim.API.Controllers;

public class AuthController : BaseApiController
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var ipAddress = GetClientIpAddress();
        var fullCommand = command with { IpAddress = ipAddress };
        var response = await Mediator.Send(fullCommand);

        SetRefreshTokenCookie(response.RefreshToken, response.ExpiresAt);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token tidak ditemukan di cookie." });
        }

        var ipAddress = GetClientIpAddress();
        var command = new RefreshTokenCommand(refreshToken, ipAddress);
        var response = await Mediator.Send(command);

        SetRefreshTokenCookie(response.RefreshToken, response.ExpiresAt);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await Mediator.Send(new LogoutCommand(refreshToken));
        }

        Response.Cookies.Delete(RefreshTokenCookieName);
        return NoContent();
    }

    private void SetRefreshTokenCookie(string token, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            Path = "/api/auth"
        };

        Response.Cookies.Append(RefreshTokenCookieName, token, cookieOptions);
    }

    private string? GetClientIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedHeader))
        {
            return forwardedHeader.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
