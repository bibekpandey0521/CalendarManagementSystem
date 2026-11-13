CREATE PROCEDURE dbo.usp_EmailNotificationLogs_Create
    @MeetingId INT,
    @ParticipantId INT = NULL,
    @RecipientEmail NVARCHAR(200),
    @NotificationType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for duplicate notification
    IF EXISTS (SELECT 1 FROM dbo.EmailNotificationLogs 
               WHERE MeetingId = @MeetingId AND ParticipantId = @ParticipantId 
               AND NotificationType = @NotificationType AND Status IN ('Pending', 'Sent'))
    BEGIN
        RETURN;
    END;

    INSERT INTO dbo.EmailNotificationLogs (MeetingId, ParticipantId, RecipientEmail, NotificationType, Status, AttemptCount, CreatedAt)
    VALUES (@MeetingId, @ParticipantId, @RecipientEmail, @NotificationType, 'Pending', 0, SYSUTCDATETIME());

    SELECT SCOPE_IDENTITY() AS LogId;
END;
GO
