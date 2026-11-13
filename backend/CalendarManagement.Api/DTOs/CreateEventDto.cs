using System.ComponentModel.DataAnnotations;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Payload used to create a new calendar event or meeting.
/// </summary>
public sealed class CreateEventDto
{
    /// <summary>Event title (2-200 characters).</summary>
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional detailed description (max 2000 characters).</summary>
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    /// <summary>Event start date and time.</summary>
    [Required(ErrorMessage = "Start time is required.")]
    public DateTime StartTime { get; set; }

    /// <summary>Event end date and time.</summary>
    [Required(ErrorMessage = "End time is required.")]
    public DateTime EndTime { get; set; }

    /// <summary>Whether the event spans the whole day.</summary>
    public bool IsAllDay { get; set; }

    /// <summary>Optional physical or virtual location (max 300 characters).</summary>
    [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters.")]
    public string? Location { get; set; }

    /// <summary>Event type (event or meeting). Defaults to event.</summary>
    public EventType Type { get; set; } = EventType.Event;

    /// <summary>Event category. Defaults to work.</summary>
    public EventCategory Category { get; set; } = EventCategory.Work;

    /// <summary>Initial status. Defaults to confirmed.</summary>
    public EventStatus Status { get; set; } = EventStatus.Confirmed;

    /// <summary>Reminder lead time in minutes (0-10080).</summary>
    [Range(0, 10080, ErrorMessage = "Reminder must be between 0 and 10080 minutes (7 days).")]
    public int? ReminderMinutesBefore { get; set; }
}