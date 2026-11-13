using System.ComponentModel.DataAnnotations;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Payload used to request a password reset email.
/// </summary>
public sealed class ForgotPasswordRequest
{
    /// <summary>Account email address.</summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Payload used to reset a password with a valid token.
/// </summary>
public sealed class ResetPasswordRequest
{
    /// <summary>Account email address.</summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Password reset token.</summary>
    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; } = string.Empty;

    /// <summary>New password (6-100 characters).</summary>
    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>Confirmation of the new password.</summary>
    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}