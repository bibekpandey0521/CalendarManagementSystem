using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Bindable query string values for filtering, searching, sorting and paging.
/// </summary>
public sealed class EventQueryParameters
{
    public string? Search { get; set; }

    /// <summary>Only events that finish on/after this instant.</summary>
    public DateTime? From { get; set; }

    /// <summary>Only events that start on/before this instant.</summary>
    public DateTime? To { get; set; }

    public EventType? Type { get; set; }

    public EventCategory? Category { get; set; }

    public EventStatus? Status { get; set; }

    public EventScope Scope { get; set; } = EventScope.All;

    public bool? HasReminder { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string SortBy { get; set; } = "startTime";

    public string SortDirection { get; set; } = "asc";
}
