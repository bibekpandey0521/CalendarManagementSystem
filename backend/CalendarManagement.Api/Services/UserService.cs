using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Data;
using CalendarManagement.Api.Exceptions;
using CalendarManagement.Api.Interfaces;
using Microsoft.Data.SqlClient;

namespace CalendarManagement.Api.Services;

public sealed class UserService : IUserService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Email, FirstName, LastName, IsEmailVerified, IsActive, CreatedAt, UpdatedAt FROM dbo.Users WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new NotFoundException("User", userId);
        }

        return new UserProfileDto
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            LastName = reader.GetString(reader.GetOrdinal("LastName")),
            IsEmailVerified = reader.GetBoolean(reader.GetOrdinal("IsEmailVerified")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }

    public async Task UpdateProfileAsync(int userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_Users_UpdateProfile @UserId, @FirstName, @LastName";
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@FirstName", dto.FirstName);
        command.Parameters.AddWithValue("@LastName", dto.LastName);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        // Verify current password
        await using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "SELECT PasswordHash FROM dbo.Users WHERE Id = @Id";
        checkCommand.Parameters.AddWithValue("@Id", userId);

        await using var reader = await checkCommand.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new NotFoundException("User", userId);
        }

        var currentHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

        // In a real app, verify the current password hash here
        // For now, we just update the password
        await using var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = "EXEC dbo.usp_Users_ChangePassword @UserId, @NewPasswordHash";
        updateCommand.Parameters.AddWithValue("@UserId", userId);
        updateCommand.Parameters.AddWithValue("@NewPasswordHash", $"hashed_{dto.NewPassword}");

        await updateCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<UserSettingsDto> GetSettingsAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_Users_GetSettings @UserId";
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            // Return default settings if not found
            return new UserSettingsDto
            {
                UserId = userId,
                Theme = "light",
                NotificationsEnabled = true,
                EmailRemindersEnabled = true
            };
        }

        return new UserSettingsDto
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            Theme = reader.GetString(reader.GetOrdinal("Theme")),
            NotificationsEnabled = reader.GetBoolean(reader.GetOrdinal("NotificationsEnabled")),
            EmailRemindersEnabled = reader.GetBoolean(reader.GetOrdinal("EmailRemindersEnabled"))
        };
    }

    public async Task UpdateSettingsAsync(int userId, UpdateSettingsDto dto, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_Users_UpdateSettings @UserId, @Theme, @NotificationsEnabled, @EmailRemindersEnabled";
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@Theme", dto.Theme);
        command.Parameters.AddWithValue("@NotificationsEnabled", dto.NotificationsEnabled);
        command.Parameters.AddWithValue("@EmailRemindersEnabled", dto.EmailRemindersEnabled);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAccountAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_Users_Delete @UserId";
        command.Parameters.AddWithValue("@UserId", userId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
