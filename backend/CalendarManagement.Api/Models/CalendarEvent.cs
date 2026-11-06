namespace CalendarManagement.Api.Models;

/// <summary>
/// A single entry on the calendar. A meeting is simply an event with
/// <see cref="Type"/> set to <see cref="EventType.Meeting"/>.
/// </summary>
public class CalendarEvent
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>Stored in UTC.</summary>
    public DateTime StartTime { get; set; }

    /// <summary>Stored in UTC.</summary>
    public DateTime EndTime { get; set; }

    public bool IsAllDay { get; set; }

    public string? Location { get; set; }

    public EventType Type { get; set; }

    public EventCategory Category { get; set; }

    public EventStatus Status { get; set; }

    public int EventTypeId { get; set; }

    public int EventCategoryId { get; set; }

    public int EventStatusId { get; set; }

    /// <summary>How many minutes before the event a reminder should fire.</summary>
    public int? ReminderMinutesBefore { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
