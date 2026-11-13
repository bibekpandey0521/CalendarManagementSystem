using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Data;
using CalendarManagement.Api.Exceptions;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Services.Email;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CalendarManagement.Api.Services.Authentication;

public interface IPasswordResetService
{
    Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}

public sealed class PasswordResetService : IPasswordResetService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        IDbConnectionFactory connectionFactory,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IConfiguration configuration,
        ILogger<PasswordResetService> logger)
    {
        _connectionFactory = connectionFactory;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        // Find the user by email (without revealing if account exists)
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Email, FirstName FROM dbo.Users WHERE Email = @Email";
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            // User not found - log but return success to prevent email enumeration
            _logger.LogInformation("Password reset requested for non-existent email: {Email}", email);
            return;
        }

        var userId = reader.GetInt32(reader.GetOrdinal("Id"));
        var userEmail = reader.GetString(reader.GetOrdinal("Email"));
        var firstName = reader.GetString(reader.GetOrdinal("FirstName"));
        await reader.CloseAsync();

        // Generate a cryptographically secure token
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var plainToken = Convert.ToBase64String(tokenBytes);
        var tokenHash = HashToken(plainToken);

        // Create token in database (invalidates old tokens)
        await using var createCommand = connection.CreateCommand();
        createCommand.CommandText = "EXEC dbo.usp_PasswordResetTokens_Create @UserId, @TokenHash, @ExpiresAt";
        createCommand.Parameters.AddWithValue("@UserId", userId);
        createCommand.Parameters.AddWithValue("@TokenHash", tokenHash);
        createCommand.Parameters.AddWithValue("@ExpiresAt", DateTime.UtcNow.AddMinutes(30));

        await createCommand.ExecuteNonQueryAsync(cancellationToken);

        // Generate reset URL
        var frontendBaseUrl = _configuration["FRONTEND_BASE_URL"] ?? _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
        var resetUrl = $"{frontendBaseUrl}/reset-password?email={Uri.EscapeDataString(userEmail)}&token={Uri.EscapeDataString(plainToken)}";

        // Generate email template
        var emailHtml = _emailTemplateService.GeneratePasswordResetEmail(userEmail, resetUrl);

        // Send email
        try
        {
            await _emailService.SendEmailAsync(userEmail, "Password Reset Request – Calendar Management System", emailHtml, cancellationToken);
            _logger.LogInformation("Password reset email sent to {Email}", userEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", userEmail);
            throw;
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        // Hash the token from the request
        var tokenHash = HashToken(request.Token);

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        // Find the token
        await using var command = connection.CreateCommand();
        command.CommandText = "EXEC dbo.usp_PasswordResetTokens_GetByHash @TokenHash";
        command.Parameters.AddWithValue("@TokenHash", tokenHash);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            _logger.LogWarning("Password reset attempted with invalid token for email: {Email}", request.Email);
            throw new DomainValidationException("Invalid or expired reset token.");
        }

        var tokenId = reader.GetInt32(reader.GetOrdinal("Id"));
        var userId = reader.GetInt32(reader.GetOrdinal("UserId"));
        var expiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt"));
        var isUsed = reader.GetBoolean(reader.GetOrdinal("IsUsed"));
        await reader.CloseAsync();

        // Validate token
        if (isUsed)
        {
            _logger.LogWarning("Password reset attempted with already used token for email: {Email}", request.Email);
            throw new DomainValidationException("This reset token has already been used.");
        }

        if (expiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Password reset attempted with expired token for email: {Email}", request.Email);
            throw new DomainValidationException("This reset token has expired.");
        }

        // Verify the user email matches
        await using var userCommand = connection.CreateCommand();
        userCommand.CommandText = "SELECT Email FROM dbo.Users WHERE Id = @Id";
        userCommand.Parameters.AddWithValue("@Id", userId);

        await using var userReader = await userCommand.ExecuteReaderAsync(cancellationToken);
        if (!await userReader.ReadAsync(cancellationToken))
        {
            throw new NotFoundException("User", userId);
        }

        var userEmail = userReader.GetString(userReader.GetOrdinal("Email"));
        await userReader.CloseAsync();

        if (!string.Equals(userEmail, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Password reset attempted with mismatched email: {RequestEmail} vs {UserEmail}", request.Email, userEmail);
            throw new DomainValidationException("Invalid reset token.");
        }

        // Update password
        var newPasswordHash = $"hashed_{request.NewPassword}";

        await using var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = "EXEC dbo.usp_Users_ChangePassword @UserId, @NewPasswordHash";
        updateCommand.Parameters.AddWithValue("@UserId", userId);
        updateCommand.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);

        await updateCommand.ExecuteNonQueryAsync(cancellationToken);

        // Mark token as used
        await using var markCommand = connection.CreateCommand();
        markCommand.CommandText = "EXEC dbo.usp_PasswordResetTokens_MarkAsUsed @TokenId";
        markCommand.Parameters.AddWithValue("@TokenId", tokenId);

        await markCommand.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Password reset successful for user {UserId}", userId);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
