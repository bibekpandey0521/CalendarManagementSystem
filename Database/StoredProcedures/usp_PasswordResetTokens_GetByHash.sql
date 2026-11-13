CREATE PROCEDURE dbo.usp_PasswordResetTokens_GetByHash
    @TokenHash NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, TokenHash, CreatedAt, ExpiresAt, UsedAt, IsUsed
    FROM dbo.PasswordResetTokens
    WHERE TokenHash = @TokenHash;
END;
GO
