using System.ComponentModel.DataAnnotations;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.DTOs;

public sealed class EventDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTime StartTime { get; init; }

    public DateTime EndTime { get; init; }

    public bool IsAllDay { get; init; }

    public string? Location { get; init; }

    public EventType Type { get; init; }

    public EventCategory Category { get; init; }

    public EventStatus Status { get; init; }

    public int? ReminderMinutesBefore { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
