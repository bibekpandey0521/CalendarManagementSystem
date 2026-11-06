namespace CalendarManagement.Api.Models;

/// <summary>
/// Distinguishes a general calendar event from a scheduled meeting.
/// Values match the seeded rows of dbo.EventTypes.
/// </summary>
public enum EventType
{
    Event = 1,
    Meeting = 2
}

/// <summary>
/// Where the event sits in its lifecycle. Values match dbo.EventStatuses.
/// </summary>
public enum EventStatus
{
    Confirmed = 1,
    Tentative = 2,
    Cancelled = 3
}

/// <summary>
/// Used by the UI for colour coding and filtering. Values match dbo.EventCategories.
/// </summary>
public enum EventCategory
{
    Work = 1,
    Personal = 2,
    Birthday = 3,
    Holiday = 4,
    Other = 5
}

/// <summary>
/// Query scope used when listing events.
/// </summary>
public enum EventScope
{
    All = 0,
    Upcoming = 1,
    Past = 2
}
