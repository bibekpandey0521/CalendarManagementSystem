CREATE PROCEDURE dbo.usp_EmailNotificationLogs_UpdateStatus
    @LogId INT,
    @Status NVARCHAR(20),
    @AttemptCount INT,
    @ErrorMessage NVARCHAR(2000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.EmailNotificationLogs
    SET Status = @Status,
        AttemptCount = @AttemptCount,
        SentAt = CASE WHEN @Status = 'Sent' THEN SYSUTCDATETIME() ELSE SentAt END,
        ErrorMessage = @ErrorMessage
    WHERE Id = @LogId;
END;
GO
