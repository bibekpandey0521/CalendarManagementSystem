using CalendarManagement.Api.DTOs;

namespace CalendarManagement.Api.Interfaces;

public interface IMeetingParticipantRepository
{
    Task<int> CreateAsync(int meetingId, string email, string? participantName = null, int? userId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MeetingParticipantDto>> GetByMeetingIdAsync(int meetingId, CancellationToken cancellationToken = default);
    Task DeleteByMeetingIdAsync(int meetingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MeetingParticipantDto>> GetByMeetingIdsAsync(IEnumerable<int> meetingIds, CancellationToken cancellationToken = default);
}

public interface IEmailNotificationLogRepository
{
    Task<int> CreateAsync(int meetingId, int? participantId, string recipientEmail, string notificationType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailNotificationLogDto>> GetPendingAsync(int limit = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailNotificationLogDto>> GetByMeetingIdAsync(int meetingId, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(int logId, string status, int attemptCount, string? errorMessage = null, CancellationToken cancellationToken = default);
}
