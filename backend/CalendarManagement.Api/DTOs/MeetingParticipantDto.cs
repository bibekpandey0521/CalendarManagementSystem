using System.ComponentModel.DataAnnotations;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Read model for a meeting participant.
/// </summary>
public sealed class MeetingParticipantDto
{
    /// <summary>Unique participant identifier.</summary>
    public int Id { get; set; }

    /// <summary>Identifier of the meeting the participant belongs to.</summary>
    public int MeetingId { get; set; }

    /// <summary>Optional linked user identifier.</summary>
    public int? UserId { get; set; }

    /// <summary>Participant display name.</summary>
    public string? ParticipantName { get; set; }

    /// <summary>Participant email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Timestamp when the participant was added.</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Payload used to add a participant to a meeting.
/// </summary>
public sealed class CreateMeetingParticipantDto
{
    /// <summary>Participant email address.</summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Optional participant display name.</summary>
    public string? ParticipantName { get; set; }

    /// <summary>Optional linked user identifier.</summary>
    public int? UserId { get; set; }
}

/// <summary>
/// Read model for an email notification log entry.
/// </summary>
public sealed class EmailNotificationLogDto
{
    /// <summary>Unique log identifier.</summary>
    public int Id { get; set; }

    /// <summary>Identifier of the related meeting.</summary>
    public int MeetingId { get; set; }

    /// <summary>Optional participant identifier.</summary>
    public int? ParticipantId { get; set; }

    /// <summary>Recipient email address.</summary>
    public string RecipientEmail { get; set; } = string.Empty;

    /// <summary>Notification type (for example MeetingInvitation).</summary>
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>Processing status (Pending, Sent, Failed).</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Number of delivery attempts.</summary>
    public int AttemptCount { get; set; }

    /// <summary>Timestamp when the notification was sent.</summary>
    public DateTime? SentAt { get; set; }

    /// <summary>Error message when delivery fails.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Timestamp when the log entry was created.</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Payload used to update notification processing status.
/// </summary>
public sealed class UpdateNotificationStatusDto
{
    /// <summary>New processing status.</summary>
    [Required]
    public string Status { get; set; } = string.Empty;

    /// <summary>Number of delivery attempts.</summary>
    public int AttemptCount { get; set; }

    /// <summary>Error message when delivery fails.</summary>
    public string? ErrorMessage { get; set; }
}