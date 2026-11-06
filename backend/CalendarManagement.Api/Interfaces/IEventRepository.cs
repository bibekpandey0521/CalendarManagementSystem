using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.Interfaces;

public interface IEventRepository
{
    Task<PagedResult<CalendarEvent>> GetPagedAsync(
        EventQueryParameters parameters,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<CalendarEvent?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CalendarEvent>> GetUpcomingAsync(
        DateTime now,
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CalendarEvent>> GetPastAsync(
        DateTime now,
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default);

    Task<EventStatistics> GetStatisticsAsync(DateTime now, CancellationToken cancellationToken = default);

    Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(CancellationToken cancellationToken = default);

    Task<bool> IsOverlappingAsync(
        int? calendarEventId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);
}
