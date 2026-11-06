using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Mappings;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IEventRepository _eventRepository;

    public DashboardService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var statistics = await _eventRepository.GetStatisticsAsync(DateTime.UtcNow, cancellationToken);
        var upcoming = await _eventRepository.GetUpcomingAsync(DateTime.UtcNow, 5, meetingsOnly: false, cancellationToken);
        var nextMeeting = upcoming.FirstOrDefault();

        return new DashboardSummaryDto
        {
            TotalEvents = statistics.TotalEvents,
            UpcomingEvents = statistics.UpcomingEvents,
            TodaysEvents = statistics.TodaysEvents,
            CompletedEvents = statistics.CompletedEvents,
            RemindersSet = statistics.RemindersSet,
            UpcomingMeetings = statistics.UpcomingMeetings,
            CompletedMeetings = statistics.CompletedMeetings,
            NextMeeting = nextMeeting is null ? null : nextMeeting.ToDto()
        };
    }
}
