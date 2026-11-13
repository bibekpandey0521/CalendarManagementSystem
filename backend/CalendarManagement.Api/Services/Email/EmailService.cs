using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace CalendarManagement.Api.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

public sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var smtpHost = _configuration["SMTP_HOST"] ?? _configuration["Smtp:Host"];
        var smtpPort = int.Parse(_configuration["SMTP_PORT"] ?? _configuration["Smtp:Port"] ?? "587");
        var smtpUsername = _configuration["SMTP_USERNAME"] ?? _configuration["Smtp:Username"];
        var smtpPassword = _configuration["SMTP_PASSWORD"] ?? _configuration["Smtp:Password"];
        var fromEmail = _configuration["SMTP_FROM_EMAIL"] ?? _configuration["Smtp:FromEmail"];
        var fromName = _configuration["SMTP_FROM_NAME"] ?? _configuration["Smtp:FromName"];

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
        {
            _logger.LogWarning("SMTP configuration is missing. Email will be logged instead of sent (development mode).");
            _logger.LogInformation("DEVELOPMENT MODE - Password reset email would be sent to {Email}", toEmail);
            _logger.LogInformation("DEVELOPMENT MODE - Subject: {Subject}", subject);
            _logger.LogInformation("DEVELOPMENT MODE - Body: {Body}", htmlBody);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName ?? "Calendar Management", fromEmail ?? smtpUsername));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(smtpUsername, smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
