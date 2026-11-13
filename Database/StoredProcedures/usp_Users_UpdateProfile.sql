CREATE PROCEDURE dbo.usp_Users_UpdateProfile
    @UserId INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET FirstName = @FirstName,
        LastName = @LastName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @UserId;
END;
GO
