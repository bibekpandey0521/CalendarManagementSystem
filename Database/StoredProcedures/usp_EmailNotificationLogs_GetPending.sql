CREATE PROCEDURE dbo.usp_EmailNotificationLogs_GetPending
    @Limit INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Limit) Id, MeetingId, ParticipantId, RecipientEmail, NotificationType, 
           Status, AttemptCount, SentAt, ErrorMessage, CreatedAt
    FROM dbo.EmailNotificationLogs
    WHERE Status IN ('Pending', 'Retry')
    ORDER BY CreatedAt ASC;
END;
GO
