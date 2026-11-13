CREATE PROCEDURE dbo.usp_PasswordResetTokens_MarkAsUsed
    @TokenId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PasswordResetTokens
    SET IsUsed = 1, UsedAt = SYSUTCDATETIME()
    WHERE Id = @TokenId;
END;
GO
