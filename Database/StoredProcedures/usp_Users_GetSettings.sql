CREATE PROCEDURE dbo.usp_Users_GetSettings
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, Theme, NotificationsEnabled, EmailRemindersEnabled, UpdatedAt
    FROM dbo.UserSettings
    WHERE UserId = @UserId;
END;
GO
