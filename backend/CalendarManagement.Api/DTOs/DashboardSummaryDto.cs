namespace CalendarManagement.Api.DTOs;

public sealed class DashboardSummaryDto
{
    public int TotalEvents { get; init; }

    public int UpcomingEvents { get; init; }

    public int TodaysEvents { get; init; }

    public int CompletedEvents { get; init; }

    public int RemindersSet { get; init; }

    public int UpcomingMeetings { get; init; }

    public int CompletedMeetings { get; init; }

    public EventDto? NextMeeting { get; init; }
}
