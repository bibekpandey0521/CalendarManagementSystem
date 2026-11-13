CREATE TABLE dbo.PasswordResetTokens
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PasswordResetTokens PRIMARY KEY,
    UserId INT NOT NULL CONSTRAINT FK_PasswordResetTokens_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),
    TokenHash NVARCHAR(64) NOT NULL CONSTRAINT UQ_PasswordResetTokens_TokenHash UNIQUE,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_PasswordResetTokens_CreatedAt DEFAULT SYSUTCDATETIME(),
    ExpiresAt DATETIME2(7) NOT NULL,
    UsedAt DATETIME2(7) NULL,
    IsUsed BIT NOT NULL CONSTRAINT DF_PasswordResetTokens_IsUsed DEFAULT (0)
);
GO

CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_UserId ON dbo.PasswordResetTokens (UserId);
GO

CREATE NONCLUSTERED INDEX IX_PasswordResetTokens_ExpiresAt ON dbo.PasswordResetTokens (ExpiresAt) WHERE IsUsed = 0;
GO
