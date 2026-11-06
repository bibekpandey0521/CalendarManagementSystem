using System.Data;
using CalendarManagement.Api.Data;
using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Models;
using Microsoft.Data.SqlClient;

namespace CalendarManagement.Api.Repositories;

public sealed class EventRepository : IEventRepository
{
    private const string SelectAllProcedure = "usp_CalendarEvent_SelectAll";
    private const string GetByIdProcedure = "usp_CalendarEvent_GetById";
    private const string CreateProcedure = "usp_CalendarEvent_Create";
    private const string UpdateProcedure = "usp_CalendarEvent_Update";
    private const string DeleteProcedure = "usp_CalendarEvent_Delete";
    private const string UpcomingProcedure = "usp_CalendarEvent_GetUpcoming";
    private const string PastProcedure = "usp_CalendarEvent_GetPast";
    private const string StatisticsProcedure = "usp_CalendarEvent_GetStatistics";

    private readonly IDbConnectionFactory _connectionFactory;

    public EventRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<CalendarEvent>> GetPagedAsync(
        EventQueryParameters parameters,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, SelectAllProcedure, cancellationToken);

        AddParameter(command, "@FromDate", parameters.From is null ? DBNull.Value : (object)parameters.From.Value.ToUniversalTime(), SqlDbType.DateTime2);
        AddParameter(command, "@ToDate", parameters.To is null ? DBNull.Value : (object)parameters.To.Value.ToUniversalTime(), SqlDbType.DateTime2);
        AddParameter(command, "@EventTypeId", parameters.Type is null ? DBNull.Value : (object)(int)parameters.Type.Value);
        AddParameter(command, "@EventCategoryId", parameters.Category is null ? DBNull.Value : (object)(int)parameters.Category.Value);
        AddParameter(command, "@EventStatusId", parameters.Status is null ? DBNull.Value : (object)(int)parameters.Status.Value);
        AddParameter(command, "@HasReminder", parameters.HasReminder, SqlDbType.Bit);
        AddParameter(command, "@Scope", (int)parameters.Scope);
        AddParameter(command, "@Now", now, SqlDbType.DateTime2);
        AddParameter(command, "@Search", string.IsNullOrWhiteSpace(parameters.Search) ? DBNull.Value : (object)parameters.Search!.Trim(), SqlDbType.NVarChar, 200);
        AddParameter(command, "@Page", parameters.Page <= 1 ? 1 : parameters.Page);
        AddParameter(command, "@PageSize", parameters.PageSize is < 1 or > 500 ? 20 : parameters.PageSize);
        AddParameter(command, "@SortBy", ResolveSortBy(parameters.SortBy));
        AddParameter(command, "@SortDirection", string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? 1 : 0);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var items = new List<CalendarEvent>();
        var totalCount = 0;

        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(MapEvent(reader));
            totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
        }

        return new PagedResult<CalendarEvent>
        {
            Items = items,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CalendarEvent?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, GetByIdProcedure, cancellationToken);
        AddParameter(command, "@Id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapEvent(reader);
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetUpcomingAsync(
        DateTime now,
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, UpcomingProcedure, cancellationToken);
        AddParameter(command, "@Now", now, SqlDbType.DateTime2);
        AddParameter(command, "@Limit", limit);
        AddParameter(command, "@EventTypeId", meetingsOnly ? (object)(int)EventType.Meeting : DBNull.Value);

        var results = new List<CalendarEvent>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(MapEvent(reader));
        }

        return results;
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetPastAsync(
        DateTime now,
        int limit,
        bool meetingsOnly,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, PastProcedure, cancellationToken);
        AddParameter(command, "@Now", now, SqlDbType.DateTime2);
        AddParameter(command, "@Limit", limit);
        AddParameter(command, "@EventTypeId", meetingsOnly ? (object)(int)EventType.Meeting : DBNull.Value);

        var results = new List<CalendarEvent>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(MapEvent(reader));
        }

        return results;
    }

    public async Task<EventStatistics> GetStatisticsAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, StatisticsProcedure, cancellationToken);
        AddParameter(command, "@Now", now, SqlDbType.DateTime2);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return new EventStatistics(0, 0, 0, 0, 0, 0, 0);
        }

        return new EventStatistics(
            reader.GetInt32(reader.GetOrdinal("TotalEvents")),
            reader.GetInt32(reader.GetOrdinal("UpcomingEvents")),
            reader.GetInt32(reader.GetOrdinal("TodaysEvents")),
            reader.GetInt32(reader.GetOrdinal("CompletedEvents")),
            reader.GetInt32(reader.GetOrdinal("RemindersSet")),
            reader.GetInt32(reader.GetOrdinal("UpcomingMeetings")),
            reader.GetInt32(reader.GetOrdinal("CompletedMeetings")));
    }

    public async Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, CreateProcedure, cancellationToken);

        AddParameter(command, "@Title", calendarEvent.Title, SqlDbType.NVarChar, 200);
        AddParameter(command, "@Description", calendarEvent.Description, SqlDbType.NVarChar, 2000);
        AddParameter(command, "@StartTime", calendarEvent.StartTime, SqlDbType.DateTime2);
        AddParameter(command, "@EndTime", calendarEvent.EndTime, SqlDbType.DateTime2);
        AddParameter(command, "@IsAllDay", calendarEvent.IsAllDay, SqlDbType.Bit);
        AddParameter(command, "@Location", calendarEvent.Location, SqlDbType.NVarChar, 300);
        AddParameter(command, "@EventTypeId", calendarEvent.EventTypeId);
        AddParameter(command, "@EventCategoryId", calendarEvent.EventCategoryId);
        AddParameter(command, "@EventStatusId", calendarEvent.EventStatusId);
        AddParameter(command, "@ReminderMinutesBefore", calendarEvent.ReminderMinutesBefore);
        AddParameter(command, "@CreatedAt", calendarEvent.CreatedAt, SqlDbType.DateTime2);
        AddParameter(command, "@UpdatedAt", calendarEvent.UpdatedAt, SqlDbType.DateTime2);
        AddParameter(command, "@CalendarEventId", DBNull.Value, SqlDbType.Int, direction: ParameterDirection.Output);

        await command.ExecuteNonQueryAsync(cancellationToken);

        calendarEvent.Id = Convert.ToInt32(command.Parameters["@CalendarEventId"].Value);

        return calendarEvent;
    }

    public async Task<bool> UpdateAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, UpdateProcedure, cancellationToken);

        AddParameter(command, "@Id", calendarEvent.Id);
        AddParameter(command, "@Title", calendarEvent.Title, SqlDbType.NVarChar, 200);
        AddParameter(command, "@Description", calendarEvent.Description, SqlDbType.NVarChar, 2000);
        AddParameter(command, "@StartTime", calendarEvent.StartTime, SqlDbType.DateTime2);
        AddParameter(command, "@EndTime", calendarEvent.EndTime, SqlDbType.DateTime2);
        AddParameter(command, "@IsAllDay", calendarEvent.IsAllDay, SqlDbType.Bit);
        AddParameter(command, "@Location", calendarEvent.Location, SqlDbType.NVarChar, 300);
        AddParameter(command, "@EventTypeId", calendarEvent.EventTypeId);
        AddParameter(command, "@EventCategoryId", calendarEvent.EventCategoryId);
        AddParameter(command, "@EventStatusId", calendarEvent.EventStatusId);
        AddParameter(command, "@ReminderMinutesBefore", calendarEvent.ReminderMinutesBefore);
        AddParameter(command, "@UpdatedAt", DateTime.UtcNow, SqlDbType.DateTime2);

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        // SET NOCOUNT ON in stored procedures suppresses the affected-rows count,
        // so ExecuteNonQuery returns -1; fall back to @@ROWCOUNT in that case.
        if (affectedRows == -1)
        {
            await using var countCmd = connection.CreateCommand();
            countCmd.CommandText = "SELECT @@ROWCOUNT";
            affectedRows = Convert.ToInt32(await countCmd.ExecuteScalarAsync(cancellationToken) ?? 0);
        }
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateCommand(connection, DeleteProcedure, cancellationToken);
        AddParameter(command, "@Id", id);

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        // SET NOCOUNT ON in stored procedures suppresses the affected-rows count,
        // so ExecuteNonQuery returns -1; fall back to @@ROWCOUNT in that case.
        if (affectedRows == -1)
        {
            await using var countCmd = connection.CreateCommand();
            countCmd.CommandText = "SELECT @@ROWCOUNT";
            affectedRows = Convert.ToInt32(await countCmd.ExecuteScalarAsync(cancellationToken) ?? 0);
        }
        return affectedRows > 0;
    }

    public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateTextCommand(connection, "SELECT COUNT(*) FROM dbo.CalendarEvents");

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result ?? 0) > 0;
    }

    public async Task<bool> IsOverlappingAsync(
        int? calendarEventId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = CreateTextCommand(connection, "SELECT dbo.fn_CalendarEvent_IsOverlapping(@StartTime, @EndTime, @CalendarEventId, @IsSelf)");
        AddParameter(command, "@StartTime", startTime, SqlDbType.DateTime2);
        AddParameter(command, "@EndTime", endTime, SqlDbType.DateTime2);
        AddParameter(command, "@CalendarEventId", calendarEventId);
        AddParameter(command, "@IsSelf", calendarEventId is null, SqlDbType.Bit);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result ?? 0) == 1;
    }

    private static SqlCommand CreateCommand(SqlConnection connection, string procedureName, CancellationToken cancellationToken)
    {
        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = procedureName;
        return command;
    }

    private static SqlCommand CreateTextCommand(SqlConnection connection, string sql)
    {
        var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = sql;
        return command;
    }

    private static void AddParameter(SqlCommand command, string name, object? value, SqlDbType? dbType = null, int size = 0, ParameterDirection direction = ParameterDirection.Input)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        if (dbType.HasValue)
        {
            parameter.SqlDbType = dbType.Value;
        }
        parameter.Direction = direction;
        parameter.Value = value ?? DBNull.Value;
        if (size > 0)
        {
            parameter.Size = size;
        }

        command.Parameters.Add(parameter);
    }

    private static int ResolveSortBy(string? sortBy)
    {
        return sortBy switch
        {
            "endTime" => 2,
            "title" => 3,
            "createdAt" => 4,
            _ => 1
        };
    }

    private static CalendarEvent MapEvent(SqlDataReader reader)
    {
        var eventTypeId = reader.GetInt32(reader.GetOrdinal("EventTypeId"));
        var eventCategoryId = reader.GetInt32(reader.GetOrdinal("EventCategoryId"));
        var eventStatusId = reader.GetInt32(reader.GetOrdinal("EventStatusId"));

        return new CalendarEvent
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
            StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
            EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
            IsAllDay = reader.GetBoolean(reader.GetOrdinal("IsAllDay")),
            Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
            Type = (EventType)Enum.Parse(typeof(EventType), reader.GetString(reader.GetOrdinal("EventTypeName")), ignoreCase: true),
            Category = (EventCategory)Enum.Parse(typeof(EventCategory), reader.GetString(reader.GetOrdinal("EventCategoryName")), ignoreCase: true),
            Status = (EventStatus)Enum.Parse(typeof(EventStatus), reader.GetString(reader.GetOrdinal("EventStatusName")), ignoreCase: true),
            EventTypeId = eventTypeId,
            EventCategoryId = eventCategoryId,
            EventStatusId = eventStatusId,
            ReminderMinutesBefore = reader.IsDBNull(reader.GetOrdinal("ReminderMinutesBefore"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("ReminderMinutesBefore")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }
}
