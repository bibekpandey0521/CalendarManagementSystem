using CalendarManagement.Api.Models;

namespace CalendarManagement.Api.DTOs;

/// <summary>
/// Bindable query string values for filtering, searching, sorting and paging events.
/// </summary>
public sealed class EventQueryParameters
{
    /// <summary>Free-text search applied to title and description.</summary>
    public string? Search { get; set; }

    /// <summary>Only events that finish on or after this instant.</summary>
    public DateTime? From { get; set; }

    /// <summary>Only events that start on or before this instant.</summary>
    public DateTime? To { get; set; }

    /// <summary>Filter by event type.</summary>
    public EventType? Type { get; set; }

    /// <summary>Filter by category.</summary>
    public EventCategory? Category { get; set; }

    /// <summary>Filter by status.</summary>
    public EventStatus? Status { get; set; }

    /// <summary>Scope of events to return (all, upcoming or past).</summary>
    public EventScope Scope { get; set; } = EventScope.All;

    /// <summary>Filter by whether a reminder is set.</summary>
    public bool? HasReminder { get; set; }

    /// <summary>Page number (1-based).</summary>
    public int Page { get; set; } = 1;

    /// <summary>Number of items per page.</summary>
    public int PageSize { get; set; } = 20;

    /// <summary>Field used for sorting (startTime by default).</summary>
    public string SortBy { get; set; } = "startTime";

    /// <summary>Sort direction (asc or desc).</summary>
    public string SortDirection { get; set; } = "asc";
}