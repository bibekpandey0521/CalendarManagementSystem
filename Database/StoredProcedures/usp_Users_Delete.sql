CREATE PROCEDURE dbo.usp_Users_Delete
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.UserSettings WHERE UserId = @UserId;
    DELETE FROM dbo.Users WHERE Id = @UserId;
END;
GO
