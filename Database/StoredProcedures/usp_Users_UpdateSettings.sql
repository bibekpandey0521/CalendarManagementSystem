CREATE PROCEDURE dbo.usp_Users_UpdateSettings
    @UserId INT,
    @Theme NVARCHAR(20),
    @NotificationsEnabled BIT,
    @EmailRemindersEnabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.UserSettings WHERE UserId = @UserId)
    BEGIN
        UPDATE dbo.UserSettings
        SET Theme = @Theme,
            NotificationsEnabled = @NotificationsEnabled,
            EmailRemindersEnabled = @EmailRemindersEnabled,
            UpdatedAt = SYSUTCDATETIME()
        WHERE UserId = @UserId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.UserSettings (UserId, Theme, NotificationsEnabled, EmailRemindersEnabled, UpdatedAt)
        VALUES (@UserId, @Theme, @NotificationsEnabled, @EmailRemindersEnabled, SYSUTCDATETIME());
    END;
END;
GO
