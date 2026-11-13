using System.ComponentModel.DataAnnotations;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Read model returned by event endpoints.
/// </summary>
public sealed class EventDto
{
    /// <summary>Unique identifier of the event.</summary>
    public int Id { get; init; }

    /// <summary>Title of the event.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Optional detailed description.</summary>
    public string? Description { get; init; }

    /// <summary>Event start date and time (UTC).</summary>
    public DateTime StartTime { get; init; }

    /// <summary>Event end date and time (UTC).</summary>
    public DateTime EndTime { get; init; }

    /// <summary>Whether the event spans the whole day.</summary>
    public bool IsAllDay { get; init; }

    /// <summary>Optional physical or virtual location.</summary>
    public string? Location { get; init; }

    /// <summary>Event type (event or meeting).</summary>
    public EventType Type { get; init; }

    /// <summary>Event category.</summary>
    public EventCategory Category { get; init; }

    /// <summary>Current event status.</summary>
    public EventStatus Status { get; init; }

    /// <summary>Reminder lead time in minutes, when configured.</summary>
    public int? ReminderMinutesBefore { get; init; }

    /// <summary>Timestamp when the event was created.</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>Timestamp when the event was last updated.</summary>
    public DateTime UpdatedAt { get; init; }
}