using System.ComponentModel.DataAnnotations;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Read model for the current user profile.
/// </summary>
public sealed class UserProfileDto
{
    /// <summary>Unique user identifier.</summary>
    public int Id { get; set; }

    /// <summary>User email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>User first name.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>User last name.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Whether the email address has been verified.</summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>Whether the account is active.</summary>
    public bool IsActive { get; set; }

    /// <summary>Timestamp when the account was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp when the account was last updated.</summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Payload used to update the user profile.
/// </summary>
public sealed class UpdateProfileDto
{
    /// <summary>Updated first name (1-100 characters).</summary>
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Updated last name (1-100 characters).</summary>
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Payload used to change the user password.
/// </summary>
public sealed class ChangePasswordDto
{
    /// <summary>Current password.</summary>
    [Required(ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>New password (6-100 characters).</summary>
    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters.")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>Confirmation of the new password.</summary>
    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Read model for user settings.
/// </summary>
public sealed class UserSettingsDto
{
    /// <summary>Settings row identifier.</summary>
    public int Id { get; set; }

    /// <summary>Identifier of the owning user.</summary>
    public int UserId { get; set; }

    /// <summary>Preferred theme (light or dark).</summary>
    public string Theme { get; set; } = "light";

    /// <summary>Whether in-app notifications are enabled.</summary>
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>Whether email reminders are enabled.</summary>
    public bool EmailRemindersEnabled { get; set; } = true;
}

/// <summary>
/// Payload used to update user settings.
/// </summary>
public sealed class UpdateSettingsDto
{
    /// <summary>Preferred theme (light or dark).</summary>
    [Required(ErrorMessage = "Theme is required.")]
    public string Theme { get; set; } = "light";

    /// <summary>Whether in-app notifications are enabled.</summary>
    [Required(ErrorMessage = "Notifications setting is required.")]
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>Whether email reminders are enabled.</summary>
    [Required(ErrorMessage = "Email reminders setting is required.")]
    public bool EmailRemindersEnabled { get; set; } = true;
}

/// <summary>
/// Payload used to request an email address change.
/// </summary>
public sealed class ChangeEmailDto
{
    /// <summary>New email address.</summary>
    [Required(ErrorMessage = "New email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string NewEmail { get; set; } = string.Empty;
}