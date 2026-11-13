using CalendarManagement.Api.Data;
using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using Microsoft.Data.SqlClient;

namespace CalendarManagement.Api.Repositories;

public sealed class MeetingParticipantRepository : IMeetingParticipantRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MeetingParticipantRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(int meetingId, string email, string? participantName = null, int? userId = null, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_MeetingParticipants_Create @MeetingId, @UserId, @ParticipantName, @Email";
        command.Parameters.AddWithValue("@MeetingId", meetingId);
        command.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
        command.Parameters.AddWithValue("@ParticipantName", (object?)participantName ?? DBNull.Value);
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(reader.GetOrdinal("ParticipantId"));
        }
        return 0; // Already exists
    }

    public async Task<IReadOnlyList<MeetingParticipantDto>> GetByMeetingIdAsync(int meetingId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_MeetingParticipants_GetByMeetingId @MeetingId";
        command.Parameters.AddWithValue("@MeetingId", meetingId);

        var results = new List<MeetingParticipantDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new MeetingParticipantDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MeetingId = reader.GetInt32(reader.GetOrdinal("MeetingId")),
                UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? null : reader.GetInt32(reader.GetOrdinal("UserId")),
                ParticipantName = reader.IsDBNull(reader.GetOrdinal("ParticipantName")) ? null : reader.GetString(reader.GetOrdinal("ParticipantName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
        return results;
    }

    public async Task DeleteByMeetingIdAsync(int meetingId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_MeetingParticipants_Delete @MeetingId";
        command.Parameters.AddWithValue("@MeetingId", meetingId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MeetingParticipantDto>> GetByMeetingIdsAsync(IEnumerable<int> meetingIds, CancellationToken cancellationToken = default)
    {
        var results = new List<MeetingParticipantDto>();
        foreach (var meetingId in meetingIds)
        {
            var participants = await GetByMeetingIdAsync(meetingId, cancellationToken);
            results.AddRange(participants);
        }
        return results;
    }
}

public sealed class EmailNotificationLogRepository : IEmailNotificationLogRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmailNotificationLogRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(int meetingId, int? participantId, string recipientEmail, string notificationType, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_EmailNotificationLogs_Create @MeetingId, @ParticipantId, @RecipientEmail, @NotificationType";
        command.Parameters.AddWithValue("@MeetingId", meetingId);
        command.Parameters.AddWithValue("@ParticipantId", (object?)participantId ?? DBNull.Value);
        command.Parameters.AddWithValue("@RecipientEmail", recipientEmail);
        command.Parameters.AddWithValue("@NotificationType", notificationType);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(reader.GetOrdinal("LogId"));
        }
        return 0; // Already exists
    }

    public async Task<IReadOnlyList<EmailNotificationLogDto>> GetPendingAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_EmailNotificationLogs_GetPending @Limit";
        command.Parameters.AddWithValue("@Limit", limit);

        var results = new List<EmailNotificationLogDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new EmailNotificationLogDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MeetingId = reader.GetInt32(reader.GetOrdinal("MeetingId")),
                ParticipantId = reader.IsDBNull(reader.GetOrdinal("ParticipantId")) ? null : reader.GetInt32(reader.GetOrdinal("ParticipantId")),
                RecipientEmail = reader.GetString(reader.GetOrdinal("RecipientEmail")),
                NotificationType = reader.GetString(reader.GetOrdinal("NotificationType")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AttemptCount = reader.GetInt32(reader.GetOrdinal("AttemptCount")),
                SentAt = reader.IsDBNull(reader.GetOrdinal("SentAt")) ? null : reader.GetDateTime(reader.GetOrdinal("SentAt")),
                ErrorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage")) ? null : reader.GetString(reader.GetOrdinal("ErrorMessage")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
        return results;
    }

    public async Task<IReadOnlyList<EmailNotificationLogDto>> GetByMeetingIdAsync(int meetingId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_EmailNotificationLogs_GetByMeetingId @MeetingId";
        command.Parameters.AddWithValue("@MeetingId", meetingId);

        var results = new List<EmailNotificationLogDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new EmailNotificationLogDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MeetingId = reader.GetInt32(reader.GetOrdinal("MeetingId")),
                ParticipantId = reader.IsDBNull(reader.GetOrdinal("ParticipantId")) ? null : reader.GetInt32(reader.GetOrdinal("ParticipantId")),
                RecipientEmail = reader.GetString(reader.GetOrdinal("RecipientEmail")),
                NotificationType = reader.GetString(reader.GetOrdinal("NotificationType")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AttemptCount = reader.GetInt32(reader.GetOrdinal("AttemptCount")),
                SentAt = reader.IsDBNull(reader.GetOrdinal("SentAt")) ? null : reader.GetDateTime(reader.GetOrdinal("SentAt")),
                ErrorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage")) ? null : reader.GetString(reader.GetOrdinal("ErrorMessage")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
        return results;
    }

    public async Task UpdateStatusAsync(int logId, string status, int attemptCount, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_EmailNotificationLogs_UpdateStatus @LogId, @Status, @AttemptCount, @ErrorMessage";
        command.Parameters.AddWithValue("@LogId", logId);
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@AttemptCount", attemptCount);
        command.Parameters.AddWithValue("@ErrorMessage", (object?)errorMessage ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
