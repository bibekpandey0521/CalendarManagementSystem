using System.ComponentModel.DataAnnotations;

namespace CalendarManagement.Api.DTOs;

public sealed class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class UpdateProfileDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
    public string LastName { get; set; } = string.Empty;
}

public sealed class ChangePasswordDto
{
    [Required(ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public sealed class UserSettingsDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Theme { get; set; } = "light";
    public bool NotificationsEnabled { get; set; } = true;
    public bool EmailRemindersEnabled { get; set; } = true;
}

public sealed class UpdateSettingsDto
{
    [Required(ErrorMessage = "Theme is required.")]
    public string Theme { get; set; } = "light";

    [Required(ErrorMessage = "Notifications setting is required.")]
    public bool NotificationsEnabled { get; set; } = true;

    [Required(ErrorMessage = "Email reminders setting is required.")]
    public bool EmailRemindersEnabled { get; set; } = true;
}

public sealed class ChangeEmailDto
{
    [Required(ErrorMessage = "New email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string NewEmail { get; set; } = string.Empty;
}
