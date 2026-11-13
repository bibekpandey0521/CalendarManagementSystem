CREATE TABLE dbo.MeetingParticipants
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MeetingParticipants PRIMARY KEY,
    MeetingId INT NOT NULL CONSTRAINT FK_MeetingParticipants_MeetingId FOREIGN KEY (MeetingId) REFERENCES dbo.CalendarEvents (Id),
    UserId INT NULL CONSTRAINT FK_MeetingParticipants_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),
    ParticipantName NVARCHAR(200) NULL,
    Email NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_MeetingParticipants_CreatedAt DEFAULT SYSUTCDATETIME()
);
GO

CREATE NONCLUSTERED INDEX IX_MeetingParticipants_MeetingId ON dbo.MeetingParticipants (MeetingId);
GO

CREATE NONCLUSTERED INDEX IX_MeetingParticipants_Email ON dbo.MeetingParticipants (Email);
GO
