using System.Globalization;
using CalendarManagement.Api.Data;
using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Exceptions;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Mappings;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.Services;

public sealed class EventService : IEventService
{
    private const int MaxReminderMinutes = 10080; // 7 days
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 500;

    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<PagedResult<EventDto>> GetEventsAsync(
        EventQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ValidateParameters(parameters);

        var now = DateTime.UtcNow;
        var pageResult = await _eventRepository.GetPagedAsync(parameters, now, cancellationToken);
        var items = pageResult.Items.Select(ToDto).ToList();

        return new PagedResult<EventDto>
        {
            Items = items,
            Page = pageResult.Page,
            PageSize = pageResult.PageSize,
            TotalCount = pageResult.TotalCount
        };
    }

    public async Task<EventDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var eventItem = await _eventRepository.GetByIdAsync(id, cancellationToken);
        if (eventItem is null)
        {
            throw new NotFoundException("CalendarEvent", id);
        }

        return eventItem.ToDto();
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto, CancellationToken cancellationToken = default)
    {
        var startTime = dto.StartTime.ToUniversalTime();
        var endTime = dto.EndTime.ToUniversalTime();
        ValidateEvent(dto.Title, startTime, endTime);

        var now = DateTime.UtcNow;
        var eventItem = new CalendarEvent
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            StartTime = startTime,
            EndTime = endTime,
            IsAllDay = dto.IsAllDay,
            Location = dto.Location?.Trim(),
            Type = dto.Type,
            Category = dto.Category,
            Status = dto.Status,
            EventTypeId = (int)dto.Type,
            EventCategoryId = (int)dto.Category,
            EventStatusId = (int)dto.Status,
            ReminderMinutesBefore = dto.ReminderMinutesBefore,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await _eventRepository.AddAsync(eventItem, cancellationToken);
        return created.ToDto();
    }

    public async Task<EventDto> UpdateAsync(int id, UpdateEventDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _eventRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new NotFoundException("CalendarEvent", id);
        }

        var startTime = dto.StartTime.ToUniversalTime();
        var endTime = dto.EndTime.ToUniversalTime();
        ValidateEvent(dto.Title, startTime, endTime);

        if (await _eventRepository.IsOverlappingAsync(
                calendarEventId: existing.Id,
                startTime: startTime,
                endTime: endTime,
                cancellationToken))
        {
            throw new DomainValidationException("An event already exists that overlaps the time range of this event.");
        }

        existing.Title = dto.Title.Trim();
        existing.Description = dto.Description?.Trim();
        existing.StartTime = startTime;
        existing.EndTime = endTime;
        existing.IsAllDay = dto.IsAllDay;
        existing.Location = dto.Location?.Trim();
        existing.Type = dto.Type;
        existing.Category = dto.Category;
        existing.Status = dto.Status;
        existing.EventTypeId = (int)dto.Type;
        existing.EventCategoryId = (int)dto.Category;
        existing.EventStatusId = (int)dto.Status;
        existing.ReminderMinutesBefore = dto.ReminderMinutesBefore;
        existing.UpdatedAt = DateTime.UtcNow;

        if (!await _eventRepository.UpdateAsync(existing, cancellationToken))
        {
            throw new NotFoundException("CalendarEvent", id);
        }

        return existing.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _eventRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("CalendarEvent", id);
        }
    }

    public async Task<IReadOnlyList<EventDto>> GetUpcomingAsync(
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetUpcomingAsync(DateTime.UtcNow, limit, meetingsOnly, cancellationToken);
        return events.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<EventDto>> GetPastAsync(
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetPastAsync(DateTime.UtcNow, limit, meetingsOnly, cancellationToken);
        return events.Select(ToDto).ToList();
    }

    internal static EventDto ToDto(CalendarEvent calendarEvent) => calendarEvent.ToDto();

    private static void ValidateParameters(EventQueryParameters parameters)
    {
        parameters.Page = parameters.Page is < 1 ? 1 : parameters.Page;
        parameters.PageSize = parameters.PageSize is < 1 or > MaxPageSize ? DefaultPageSize : parameters.PageSize;
        parameters.SortDirection = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
    }

    private static void ValidateEvent(string title, DateTime startTime, DateTime endTime)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < 2)
        {
            throw new DomainValidationException("Title must be between 2 and 200 characters.");
        }

        if (endTime <= startTime)
        {
            throw new DomainValidationException("End time must be after the start time.");
        }

        if (startTime < new DateTime(1, 1, 1) || endTime > new DateTime(9999, 12, 31))
        {
            throw new DomainValidationException("Event times fall outside the supported range.");
        }

        if (startTime.Kind != DateTimeKind.Utc || endTime.Kind != DateTimeKind.Utc)
        {
            throw new DomainValidationException("Start and end times must be provided in UTC.");
        }
    }
}
