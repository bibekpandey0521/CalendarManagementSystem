using System.ComponentModel.DataAnnotations;
using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Full replacement payload. Kept separate from the create DTO so the two can
/// evolve independently without affecting each other's validation rules.
/// </summary>
public sealed class UpdateEventDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required.")]
    public DateTime EndTime { get; set; }

    public bool IsAllDay { get; set; }

    [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters.")]
    public string? Location { get; set; }

    public EventType Type { get; set; } = EventType.Event;

    public EventCategory Category { get; set; } = EventCategory.Work;

    public EventStatus Status { get; set; } = EventStatus.Confirmed;

    [Range(0, 10080, ErrorMessage = "Reminder must be between 0 and 10080 minutes (7 days).")]
    public int? ReminderMinutesBefore { get; set; }
}
