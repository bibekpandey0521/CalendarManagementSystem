namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Aggregated dashboard counters and the next upcoming meeting.
/// </summary>
public sealed class DashboardSummaryDto
{
    /// <summary>Total number of events.</summary>
    public int TotalEvents { get; init; }

    /// <summary>Number of upcoming events.</summary>
    public int UpcomingEvents { get; init; }

    /// <summary>Number of events scheduled for today.</summary>
    public int TodaysEvents { get; init; }

    /// <summary>Number of completed events.</summary>
    public int CompletedEvents { get; init; }

    /// <summary>Number of events with reminders configured.</summary>
    public int RemindersSet { get; init; }

    /// <summary>Number of upcoming meetings.</summary>
    public int UpcomingMeetings { get; init; }

    /// <summary>Number of completed meetings.</summary>
    public int CompletedMeetings { get; init; }

    /// <summary>The next upcoming meeting, if any.</summary>
    public EventDto? NextMeeting { get; init; }
}