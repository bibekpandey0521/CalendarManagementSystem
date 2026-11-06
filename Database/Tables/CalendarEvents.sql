CREATE TABLE dbo.CalendarEvents
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CalendarEvents PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000) NULL,
    StartTime DATETIME2(7) NOT NULL,
    EndTime DATETIME2(7) NOT NULL,
    IsAllDay BIT NOT NULL CONSTRAINT DF_CalendarEvents_IsAllDay DEFAULT (0),
    Location NVARCHAR(300) NULL,
    EventTypeId INT NOT NULL,
    EventCategoryId INT NOT NULL,
    EventStatusId INT NOT NULL,
    ReminderMinutesBefore INT NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_CalendarEvents_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_CalendarEvents_UpdatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_CalendarEvents_EventType FOREIGN KEY (EventTypeId) REFERENCES dbo.EventTypes (Id),
    CONSTRAINT FK_CalendarEvents_EventCategory FOREIGN KEY (EventCategoryId) REFERENCES dbo.EventCategories (Id),
    CONSTRAINT FK_CalendarEvents_EventStatus FOREIGN KEY (EventStatusId) REFERENCES dbo.EventStatuses (Id),
    CONSTRAINT CK_CalendarEvents_TitleNotEmpty CHECK (LEN(RTRIM(LTRIM(TITLE))) > 0),
    CONSTRAINT CK_CalendarEvents_EndAfterStart CHECK (EndTime > StartTime)
);
GO
