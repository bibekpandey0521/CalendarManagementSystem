using CalendarManagement.Api.Data;
using CalendarManagement.Api.DTOs;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Services.Email;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CalendarManagement.Api.Services.Background;

public sealed class EmailNotificationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailNotificationWorker> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(30);
    private const int MaxRetries = 3;

    public EmailNotificationWorker(IServiceProvider serviceProvider, ILogger<EmailNotificationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email notification worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email notifications.");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Email notification worker stopped.");
    }

    private async Task ProcessPendingNotificationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationRepo = scope.ServiceProvider.GetRequiredService<IEmailNotificationLogRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var pendingNotifications = await notificationRepo.GetPendingAsync(50, stoppingToken);

        foreach (var notification in pendingNotifications)
        {
            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                await notificationRepo.UpdateStatusAsync(notification.Id, "Processing", notification.AttemptCount + 1, null, stoppingToken);

                // Get event details for the email
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                var calendarEvent = await eventService.GetByIdAsync(notification.MeetingId, stoppingToken);

                var subject = $"Meeting Invitation: {calendarEvent.Title}";
                var body = GenerateInvitationEmailBody(calendarEvent);

                await emailService.SendEmailAsync(notification.RecipientEmail, subject, body, stoppingToken);

                await notificationRepo.UpdateStatusAsync(notification.Id, "Sent", notification.AttemptCount + 1, null, stoppingToken);
                _logger.LogInformation("Notification sent to {Email} for meeting {MeetingId}", notification.RecipientEmail, notification.MeetingId);
            }
            catch (Exception ex)
            {
                var newAttemptCount = notification.AttemptCount + 1;
                var newStatus = newAttemptCount >= MaxRetries ? "Failed" : "Retry";
                var errorMessage = ex.Message;

                await notificationRepo.UpdateStatusAsync(notification.Id, newStatus, newAttemptCount, errorMessage, stoppingToken);
                _logger.LogError(ex, "Failed to send notification to {Email} for meeting {MeetingId}. Attempt {Attempt} of {Max}", 
                    notification.RecipientEmail, notification.MeetingId, newAttemptCount, MaxRetries);
            }
        }
    }

    private static string GenerateInvitationEmailBody(CalendarManagement.Api.DTOs.EventDto calendarEvent)
    {
        var descriptionSection = !string.IsNullOrEmpty(calendarEvent.Description)
            ? $@"                <div class=""detail-row"">
                    <span class=""detail-label"">Description:</span>
                    <span class=""detail-value"">{calendarEvent.Description}</span>
                </div>"
            : "";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Meeting Invitation</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); overflow: hidden; }}
        .header {{ background-color: #2563eb; color: #ffffff; padding: 30px 40px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; font-weight: 600; }}
        .content {{ padding: 40px; }}
        .content p {{ color: #333333; line-height: 1.6; margin: 0 0 16px; }}
        .detail-row {{ display: flex; margin: 8px 0; }}
        .detail-label {{ font-weight: 600; color: #555555; min-width: 120px; }}
        .detail-value {{ color: #333333; }}
        .footer {{ background-color: #f9fafb; padding: 20px 40px; text-align: center; font-size: 12px; color: #6b7280; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Meeting Invitation</h1>
        </div>
        <div class=""content"">
            <p>Hello,</p>
            <p>You have been invited to the following meeting:</p>
            <div style=""margin: 20px 0; padding: 20px; background-color: #f9fafb; border-radius: 6px;"">
                <div class=""detail-row"">
                    <span class=""detail-label"">Title:</span>
                    <span class=""detail-value"">{calendarEvent.Title}</span>
                </div>
                <div class=""detail-row"">
                    <span class=""detail-label"">Date:</span>
                    <span class=""detail-value"">{calendarEvent.StartTime.ToString("dddd, MMMM dd, yyyy")}</span>
                </div>
                <div class=""detail-row"">
                    <span class=""detail-label"">Start Time:</span>
                    <span class=""detail-value"">{calendarEvent.StartTime.ToString("HH:mm")} UTC</span>
                </div>
                <div class=""detail-row"">
                    <span class=""detail-label"">End Time:</span>
                    <span class=""detail-value"">{calendarEvent.EndTime.ToString("HH:mm")} UTC</span>
                </div>
                <div class=""detail-row"">
                    <span class=""detail-label"">Location:</span>
                    <span class=""detail-value"">{calendarEvent.Location ?? "Not specified"}</span>
                </div>
                {descriptionSection}
            </div>
            <p>Please respond to this invitation at your earliest convenience.</p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.UtcNow.Year} Calendar Management System. All rights reserved.</p>
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
    }
}
