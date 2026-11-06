using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.Mappings;

public static class EventMappingExtensions
{
    public static EventDto ToDto(this CalendarEvent calendarEvent)
    {
        return new EventDto
        {
            Id = calendarEvent.Id,
            Title = calendarEvent.Title,
            Description = calendarEvent.Description,
            StartTime = AsUtc(calendarEvent.StartTime),
            EndTime = AsUtc(calendarEvent.EndTime),
            IsAllDay = calendarEvent.IsAllDay,
            Location = calendarEvent.Location,
            Type = calendarEvent.Type,
            Category = calendarEvent.Category,
            Status = calendarEvent.Status,
            ReminderMinutesBefore = calendarEvent.ReminderMinutesBefore,
            CreatedAt = AsUtc(calendarEvent.CreatedAt),
            UpdatedAt = AsUtc(calendarEvent.UpdatedAt)
        };
    }

    public static IReadOnlyList<EventDto> ToDtos(this IEnumerable<CalendarEvent> events)
    {
        return events.Select(ToDto).ToList();
    }

    private static DateTime AsUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
