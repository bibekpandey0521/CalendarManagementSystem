using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.Interfaces;

public interface IEventService
{
    Task<PagedResult<EventDto>> GetEventsAsync(
        EventQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<EventDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<EventDto> CreateAsync(CreateEventDto dto, CancellationToken cancellationToken = default);

    Task<EventDto> UpdateAsync(int id, UpdateEventDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventDto>> GetUpcomingAsync(
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventDto>> GetPastAsync(
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default);
}
