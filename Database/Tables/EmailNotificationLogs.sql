CREATE TABLE dbo.EmailNotificationLogs
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmailNotificationLogs PRIMARY KEY,
    MeetingId INT NOT NULL CONSTRAINT FK_EmailNotificationLogs_MeetingId FOREIGN KEY (MeetingId) REFERENCES dbo.CalendarEvents (Id),
    ParticipantId INT NULL CONSTRAINT FK_EmailNotificationLogs_ParticipantId FOREIGN KEY (ParticipantId) REFERENCES dbo.MeetingParticipants (Id),
    RecipientEmail NVARCHAR(200) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_EmailNotificationLogs_Status DEFAULT ('Pending'),
    AttemptCount INT NOT NULL CONSTRAINT DF_EmailNotificationLogs_AttemptCount DEFAULT (0),
    SentAt DATETIME2(7) NULL,
    ErrorMessage NVARCHAR(2000) NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_EmailNotificationLogs_CreatedAt DEFAULT SYSUTCDATETIME()
);
GO

CREATE NONCLUSTERED INDEX IX_EmailNotificationLogs_MeetingId ON dbo.EmailNotificationLogs (MeetingId);
GO

CREATE NONCLUSTERED INDEX IX_EmailNotificationLogs_Status ON dbo.EmailNotificationLogs (Status) WHERE Status IN ('Pending', 'Retry');
GO

CREATE NONCLUSTERED INDEX IX_EmailNotificationLogs_RecipientEmail ON dbo.EmailNotificationLogs (RecipientEmail);
GO
