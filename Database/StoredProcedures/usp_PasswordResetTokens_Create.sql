CREATE PROCEDURE dbo.usp_PasswordResetTokens_Create
    @UserId INT,
    @TokenHash NVARCHAR(64),
    @ExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Invalidate any existing unused tokens for this user
    UPDATE dbo.PasswordResetTokens
    SET IsUsed = 1, UsedAt = SYSUTCDATETIME()
    WHERE UserId = @UserId AND IsUsed = 0;

    -- Create new token
    INSERT INTO dbo.PasswordResetTokens (UserId, TokenHash, ExpiresAt, IsUsed)
    VALUES (@UserId, @TokenHash, @ExpiresAt, 0);

    SELECT SCOPE_IDENTITY() AS TokenId;
END;
GO
