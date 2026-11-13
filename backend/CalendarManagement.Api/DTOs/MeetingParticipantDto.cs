using System.ComponentModel.DataAnnotations;

namespace CalendarManagement.Api.DTOs;

public sealed class MeetingParticipantDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public int? UserId { get; set; }
    public string? ParticipantName { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class CreateMeetingParticipantDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    public string? ParticipantName { get; set; }
    public int? UserId { get; set; }
}

public sealed class EmailNotificationLogDto
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public int? ParticipantId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class UpdateNotificationStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    public int AttemptCount { get; set; }
    public string? ErrorMessage { get; set; }
}
