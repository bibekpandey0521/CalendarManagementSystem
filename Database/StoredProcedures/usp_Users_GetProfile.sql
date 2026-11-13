CREATE PROCEDURE dbo.usp_Users_GetProfile
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Email, FirstName, LastName, IsEmailVerified, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Users
    WHERE Id = @UserId;
END;
GO
