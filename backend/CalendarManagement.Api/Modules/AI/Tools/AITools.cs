using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Modules.AI.Interfaces;
using CalendarManagement.Api.Modules.AI.Models;

namespace CalendarManagement.Api.Modules.AI.Tools;

public class GetUpcomingEventsTool : IAITool
{
    private readonly IEventService _eventService;

    public GetUpcomingEventsTool(IEventService eventService)
    {
        _eventService = eventService;
    }

    public string Name => "GetUpcomingEvents";
    public string Description => "Retrieves upcoming calendar events and meetings. Returns a list of upcoming events with their titles, times, locations, and types.";

    public async Task<AIToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var limit = parameters.TryGetValue("limit", out var l) ? Convert.ToInt32(l) : 10;
            var events = await _eventService.GetUpcomingAsync(limit, false, cancellationToken);
            return new AIToolResult
            {
                ToolName = Name,
                Success = true,
                Result = events
            };
        }
        catch (Exception ex)
        {
            return new AIToolResult
            {
                ToolName = Name,
                Success = false,
                Error = ex.Message
            };
        }
    }
}

public class GetDashboardSummaryTool : IAITool
{
    private readonly IDashboardService _dashboardService;

    public GetDashboardSummaryTool(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public string Name => "GetDashboardSummary";
    public string Description => "Retrieves dashboard summary statistics including total events, upcoming events, today's events, completed events, reminders set, upcoming meetings, and completed meetings.";

    public async Task<AIToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var summary = await _dashboardService.GetSummaryAsync(cancellationToken);
            return new AIToolResult
            {
                ToolName = Name,
                Success = true,
                Result = summary
            };
        }
        catch (Exception ex)
        {
            return new AIToolResult
            {
                ToolName = Name,
                Success = false,
                Error = ex.Message
            };
        }
    }
}

public class GetPastEventsTool : IAITool
{
    private readonly IEventService _eventService;

    public GetPastEventsTool(IEventService eventService)
    {
        _eventService = eventService;
    }

    public string Name => "GetPastEvents";
    public string Description => "Retrieves past calendar events and meetings. Returns a list of past events with their titles, times, locations, and types.";

    public async Task<AIToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var limit = parameters.TryGetValue("limit", out var l) ? Convert.ToInt32(l) : 10;
            var events = await _eventService.GetPastAsync(limit, false, cancellationToken);
            return new AIToolResult
            {
                ToolName = Name,
                Success = true,
                Result = events
            };
        }
        catch (Exception ex)
        {
            return new AIToolResult
            {
                ToolName = Name,
                Success = false,
                Error = ex.Message
            };
        }
    }
}

public class GetEventByIdTool : IAITool
{
    private readonly IEventService _eventService;

    public GetEventByIdTool(IEventService eventService)
    {
        _eventService = eventService;
    }

    public string Name => "GetEventById";
    public string Description => "Retrieves a specific calendar event by its ID. Returns the event details including title, description, time, location, type, category, and status.";

    public async Task<AIToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = Convert.ToInt32(parameters["id"]);
            var eventDto = await _eventService.GetByIdAsync(id, cancellationToken);
            return new AIToolResult
            {
                ToolName = Name,
                Success = true,
                Result = eventDto
            };
        }
        catch (Exception ex)
        {
            return new AIToolResult
            {
                ToolName = Name,
                Success = false,
                Error = ex.Message
            };
        }
    }
}

public class CreateEventTool : IAITool
{
    private readonly IEventService _eventService;

    public CreateEventTool(IEventService eventService)
    {
        _eventService = eventService;
    }

    public string Name => "CreateEvent";
    public string Description => "Creates a new calendar event or meeting. Requires title, startTime, and endTime in ISO 8601 format. Optional: description, location, type (1=Event, 2=Meeting), category (1=Work, 2=Personal, 3=Birthday, 4=Holiday, 5=Other), status (1=Confirmed, 2=Tentative, 3=Cancelled), reminderMinutesBefore (0-10080).";

    public async Task<AIToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = new CalendarManagement.Api.DTOs.CreateEventDto
            {
                Title = parameters.TryGetValue("title", out var t) ? t.ToString()! : string.Empty,
                Description = parameters.TryGetValue("description", out var d) ? d.ToString() : null,
                StartTime = parameters.TryGetValue("startTime", out var s) ? DateTime.Parse(s.ToString()!) : DateTime.UtcNow,
                EndTime = parameters.TryGetValue("endTime", out var e) ? DateTime.Parse(e.ToString()!) : DateTime.UtcNow.AddHours(1),
                IsAllDay = parameters.TryGetValue("isAllDay", out var a) && Convert.ToBoolean(a),
                Location = parameters.TryGetValue("location", out var l) ? l.ToString() : null,
                Type = parameters.TryGetValue("type", out var ty) ? (CalendarManagement.Api.Models.EventType)Convert.ToInt32(ty) : CalendarManagement.Api.Models.EventType.Event,
                Category = parameters.TryGetValue("category", out var c) ? (CalendarManagement.Api.Models.EventCategory)Convert.ToInt32(c) : CalendarManagement.Api.Models.EventCategory.Work,
                Status = parameters.TryGetValue("status", out var st) ? (CalendarManagement.Api.Models.EventStatus)Convert.ToInt32(st) : CalendarManagement.Api.Models.EventStatus.Confirmed,
                ReminderMinutesBefore = parameters.TryGetValue("reminderMinutesBefore", out var r) ? Convert.ToInt32(r) : null
            };

            var created = await _eventService.CreateAsync(dto, cancellationToken);
            return new AIToolResult
            {
                ToolName = Name,
                Success = true,
                Result = created
            };
        }
        catch (Exception ex)
        {
            return new AIToolResult
            {
                ToolName = Name,
                Success = false,
                Error = ex.Message
            };
        }
    }
}

public class DeleteEventTool : IAITool
{
    private readonly IEventService _eventService;

    public DeleteEventTool(IEventService eventService)
    {
        _eventService = eventService;
    }

    public string Name => "DeleteEvent";
    public string Description => "Deletes a calendar event by its ID. Requires the event ID.";

    public async Task<AIToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = Convert.ToInt32(parameters["id"]);
            await _eventService.DeleteAsync(id, cancellationToken);
            return new AIToolResult
            {
                ToolName = Name,
                Success = true,
                Result = new { message = $"Event {id} deleted successfully." }
            };
        }
        catch (Exception ex)
        {
            return new AIToolResult
            {
                ToolName = Name,
                Success = false,
                Error = ex.Message
            };
        }
    }
}
