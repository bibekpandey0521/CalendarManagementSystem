using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Models;
using CalendarManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CalendarManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Searches, filters, sorts and pages calendar events. Used by the calendar
    /// grid and by the search box.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EventDto>>> GetEvents(
        [FromQuery] EventQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var result = await _eventService.GetEventsAsync(parameters, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns upcoming events, optionally filtered to meetings only.</summary>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IReadOnlyList<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetUpcoming(
        [FromQuery] int limit = 5,
        [FromQuery] bool meetingsOnly = false,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventService.GetUpcomingAsync(limit, meetingsOnly, cancellationToken);
        return Ok(events);
    }

    /// <summary>Returns past events, optionally filtered to meetings only.</summary>
    [HttpGet("past")]
    [ProducesResponseType(typeof(IReadOnlyList<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetPast(
        [FromQuery] int limit = 5,
        [FromQuery] bool meetingsOnly = false,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventService.GetPastAsync(limit, meetingsOnly, cancellationToken);
        return Ok(events);
    }

    /// <summary>Returns a single event by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var eventItem = await _eventService.GetByIdAsync(id, cancellationToken);
        return Ok(eventItem);
    }

    /// <summary>Creates a new event or meeting.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventDto>> Create(
        [FromBody] CreateEventDto dto,
        CancellationToken cancellationToken = default)
    {
        var eventItem = await _eventService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = eventItem.Id }, eventItem);
    }

    /// <summary>Updates an existing event or meeting.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> Update(
        int id,
        [FromBody] UpdateEventDto dto,
        CancellationToken cancellationToken = default)
    {
        var eventItem = await _eventService.UpdateAsync(id, dto, cancellationToken);
        return Ok(eventItem);
    }

    /// <summary>Deletes an event or meeting.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _eventService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
