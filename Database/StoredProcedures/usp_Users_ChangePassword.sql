CREATE PROCEDURE dbo.usp_Users_ChangePassword
    @UserId INT,
    @NewPasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET PasswordHash = @NewPasswordHash,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @UserId;
END;
GO
