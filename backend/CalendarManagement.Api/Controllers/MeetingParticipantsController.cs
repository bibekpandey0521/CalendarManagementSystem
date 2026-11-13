using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CalendarManagement.Api.Controllers;

[ApiController]
[Route("api/meetings")]
public sealed class MeetingParticipantsController : ControllerBase
{
    private readonly IMeetingParticipantRepository _participantRepository;
    private readonly IEmailNotificationLogRepository _notificationRepository;

    public MeetingParticipantsController(
        IMeetingParticipantRepository participantRepository,
        IEmailNotificationLogRepository notificationRepository)
    {
        _participantRepository = participantRepository;
        _notificationRepository = notificationRepository;
    }

    /// <summary>Gets all participants for a meeting.</summary>
    [HttpGet("{meetingId:int}/participants")]
    [ProducesResponseType(typeof(IReadOnlyList<MeetingParticipantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MeetingParticipantDto>>> GetParticipants(
        int meetingId,
        CancellationToken cancellationToken = default)
    {
        var participants = await _participantRepository.GetByMeetingIdAsync(meetingId, cancellationToken);
        return Ok(participants);
    }

    /// <summary>Adds a participant to a meeting.</summary>
    [HttpPost("{meetingId:int}/participants")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> AddParticipant(
        int meetingId,
        [FromBody] CreateMeetingParticipantDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var participantId = await _participantRepository.CreateAsync(
            meetingId, dto.Email, dto.ParticipantName, dto.UserId, cancellationToken);

        // Create notification log
        await _notificationRepository.CreateAsync(meetingId, participantId, dto.Email, "MeetingInvitation", cancellationToken);

        return CreatedAtAction(nameof(GetParticipants), new { meetingId }, participantId);
    }

    /// <summary>Removes all participants from a meeting.</summary>
    [HttpDelete("{meetingId:int}/participants")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RemoveParticipants(
        int meetingId,
        CancellationToken cancellationToken = default)
    {
        await _participantRepository.DeleteByMeetingIdAsync(meetingId, cancellationToken);
        return NoContent();
    }

    /// <summary>Gets notification logs for a meeting.</summary>
    [HttpGet("{meetingId:int}/notifications")]
    [ProducesResponseType(typeof(IReadOnlyList<EmailNotificationLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EmailNotificationLogDto>>> GetNotifications(
        int meetingId,
        CancellationToken cancellationToken = default)
    {
        var logs = await _notificationRepository.GetByMeetingIdAsync(meetingId, cancellationToken);
        return Ok(logs);
    }

    /// <summary>Gets pending notifications for processing.</summary>
    [HttpGet("notifications/pending")]
    [ProducesResponseType(typeof(IReadOnlyList<EmailNotificationLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EmailNotificationLogDto>>> GetPendingNotifications(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var logs = await _notificationRepository.GetPendingAsync(limit, cancellationToken);
        return Ok(logs);
    }
}
