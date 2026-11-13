CREATE PROCEDURE dbo.usp_EmailNotificationLogs_GetByMeetingId
    @MeetingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, MeetingId, ParticipantId, RecipientEmail, NotificationType, 
           Status, AttemptCount, SentAt, ErrorMessage, CreatedAt
    FROM dbo.EmailNotificationLogs
    WHERE MeetingId = @MeetingId
    ORDER BY CreatedAt ASC;
END;
GO
