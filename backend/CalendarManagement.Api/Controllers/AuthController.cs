using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CalendarManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IPasswordResetService _passwordResetService;

    public AuthController(IPasswordResetService passwordResetService)
    {
        _passwordResetService = passwordResetService;
    }

    /// <summary>
    /// Requests a password reset email for the given email address.
    /// Always returns a generic response to prevent email enumeration.
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _passwordResetService.RequestPasswordResetAsync(request.Email, cancellationToken);

        // Generic response - don't reveal whether email exists
        return Ok(new
        {
            message = "If an account with that email exists, a password reset link has been sent."
        });
    }

    /// <summary>
    /// Resets the password using a valid reset token.
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _passwordResetService.ResetPasswordAsync(request, cancellationToken);

        return Ok(new
        {
            message = "Password has been reset successfully. You can now log in with your new password."
        });
    }
}
