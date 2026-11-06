namespace CalendarManagement.Api.Models;

/// <summary>
/// Aggregated numbers used by the dashboard. Calculated in a single query.
/// </summary>
public sealed record EventStatistics(
    int TotalEvents,
    int UpcomingEvents,
    int TodaysEvents,
    int CompletedEvents,
    int RemindersSet,
    int UpcomingMeetings,
    int CompletedMeetings);
