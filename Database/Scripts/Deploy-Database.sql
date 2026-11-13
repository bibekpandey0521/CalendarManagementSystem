-- Full deployment script for the live SQL Server instance.
-- Order matters: drop dependent objects first, then create the schema and seed data.

GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_SelectAll', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_SelectAll;
GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetUpcoming', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetUpcoming;
GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetPast', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetPast;
GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetStatistics', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetStatistics;
GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetById', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetById;
GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_Update', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_Update;
GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_Delete', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_Delete;
GO
IF OBJECT_ID(N'dbo.usp_CalendarEvent_Create', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_Create;
GO
IF OBJECT_ID(N'dbo.fn_CalendarEvent_IsOverlapping', N'FN') IS NOT NULL DROP FUNCTION dbo.fn_CalendarEvent_IsOverlapping;
GO
IF OBJECT_ID(N'dbo.vw_CalendarEvents_Details', N'V') IS NOT NULL DROP VIEW dbo.vw_CalendarEvents_Details;
GO
IF OBJECT_ID(N'dbo.CalendarEvents', N'U') IS NOT NULL DROP TABLE dbo.CalendarEvents;
GO
IF OBJECT_ID(N'dbo.usp_Users_Delete', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Users_Delete;
GO
IF OBJECT_ID(N'dbo.usp_Users_UpdateSettings', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Users_UpdateSettings;
GO
IF OBJECT_ID(N'dbo.usp_Users_GetSettings', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Users_GetSettings;
GO
IF OBJECT_ID(N'dbo.usp_Users_ChangePassword', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Users_ChangePassword;
GO
IF OBJECT_ID(N'dbo.usp_Users_UpdateProfile', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Users_UpdateProfile;
GO
IF OBJECT_ID(N'dbo.usp_Users_GetProfile', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_Users_GetProfile;
GO
IF OBJECT_ID(N'dbo.UserSettings', N'U') IS NOT NULL DROP TABLE dbo.UserSettings;
GO
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO
IF OBJECT_ID(N'dbo.EventStatuses', N'U') IS NOT NULL DROP TABLE dbo.EventStatuses;
GO
IF OBJECT_ID(N'dbo.EventCategories', N'U') IS NOT NULL DROP TABLE dbo.EventCategories;
GO
IF OBJECT_ID(N'dbo.EventTypes', N'U') IS NOT NULL DROP TABLE dbo.EventTypes;
GO

CREATE TABLE dbo.EventTypes
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EventTypes PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL CONSTRAINT UQ_EventTypes_Name UNIQUE
);
GO
CREATE TABLE dbo.EventCategories
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EventCategories PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL CONSTRAINT UQ_EventCategories_Name UNIQUE,
    DisplayOrder INT NOT NULL CONSTRAINT DF_EventCategories_DisplayOrder DEFAULT (0)
);
GO
CREATE TABLE dbo.EventStatuses
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EventStatuses PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL CONSTRAINT UQ_EventStatuses_Name UNIQUE
);
GO
CREATE TABLE dbo.Users
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Email NVARCHAR(200) NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsEmailVerified BIT NOT NULL CONSTRAINT DF_Users_IsEmailVerified DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Users_UpdatedAt DEFAULT SYSUTCDATETIME()
);
GO
CREATE TABLE dbo.UserSettings
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserSettings PRIMARY KEY,
    UserId INT NOT NULL CONSTRAINT FK_UserSettings_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),
    Theme NVARCHAR(20) NOT NULL CONSTRAINT DF_UserSettings_Theme DEFAULT ('light'),
    NotificationsEnabled BIT NOT NULL CONSTRAINT DF_UserSettings_NotificationsEnabled DEFAULT (1),
    EmailRemindersEnabled BIT NOT NULL CONSTRAINT DF_UserSettings_EmailRemindersEnabled DEFAULT (1),
    UpdatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_UserSettings_UpdatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_UserSettings_UserId UNIQUE (UserId)
);
GO

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
CREATE NONCLUSTERED INDEX IX_CalendarEvents_StartTime_EndTime ON dbo.CalendarEvents (StartTime, EndTime);
GO
CREATE NONCLUSTERED INDEX IX_CalendarEvents_EventTypeId ON dbo.CalendarEvents (EventTypeId);
GO
CREATE NONCLUSTERED INDEX IX_CalendarEvents_EventCategoryId ON dbo.CalendarEvents (EventCategoryId);
GO
CREATE NONCLUSTERED INDEX IX_CalendarEvents_EventStatusId ON dbo.CalendarEvents (EventStatusId);
GO
CREATE NONCLUSTERED INDEX IX_CalendarEvents_ReminderMinutesBefore ON dbo.CalendarEvents (ReminderMinutesBefore) WHERE ReminderMinutesBefore IS NOT NULL;
GO
CREATE NONCLUSTERED INDEX IX_CalendarEvents_Location ON dbo.CalendarEvents (Location) WHERE Location IS NOT NULL;
GO
CREATE NONCLUSTERED INDEX IX_CalendarEvents_IsAllDay ON dbo.CalendarEvents (IsAllDay) WHERE IsAllDay = 1;
GO
CREATE NONCLUSTERED INDEX IX_CalendarEvents_CreatedAt ON dbo.CalendarEvents (CreatedAt);
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_Create
    @Title NVARCHAR(200),
    @Description NVARCHAR(2000) = NULL,
    @StartTime DATETIME2,
    @EndTime DATETIME2,
    @IsAllDay BIT,
    @Location NVARCHAR(300) = NULL,
    @EventTypeId INT,
    @EventCategoryId INT,
    @EventStatusId INT,
    @ReminderMinutesBefore INT = NULL,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @CalendarEventId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.CalendarEvents
        (Title, Description, StartTime, EndTime, IsAllDay, Location, EventTypeId, EventCategoryId, EventStatusId, ReminderMinutesBefore, CreatedAt, UpdatedAt)
    VALUES
        (@Title, @Description, @StartTime, @EndTime, @IsAllDay, @Location, @EventTypeId, @EventCategoryId, @EventStatusId, @ReminderMinutesBefore, @CreatedAt, @UpdatedAt);

    SET @CalendarEventId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_Update
    @Id INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(2000) = NULL,
    @StartTime DATETIME2,
    @EndTime DATETIME2,
    @IsAllDay BIT,
    @Location NVARCHAR(300) = NULL,
    @EventTypeId INT,
    @EventCategoryId INT,
    @EventStatusId INT,
    @ReminderMinutesBefore INT = NULL,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CalendarEvents
    SET
        Title = @Title,
        Description = @Description,
        StartTime = @StartTime,
        EndTime = @EndTime,
        IsAllDay = @IsAllDay,
        Location = @Location,
        EventTypeId = @EventTypeId,
        EventCategoryId = @EventCategoryId,
        EventStatusId = @EventStatusId,
        ReminderMinutesBefore = @ReminderMinutesBefore,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END;
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.CalendarEvents
    WHERE Id = @Id;
END;
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.Id,
        e.Title,
        e.Description,
        e.StartTime,
        e.EndTime,
        e.IsAllDay,
        e.Location,
        e.EventTypeId,
        et.Name AS EventTypeName,
        e.EventCategoryId,
        ec.Name AS EventCategoryName,
        e.EventStatusId,
        es.Name AS EventStatusName,
        e.ReminderMinutesBefore,
        e.CreatedAt,
        e.UpdatedAt
    FROM dbo.CalendarEvents e
    INNER JOIN dbo.EventTypes et ON et.Id = e.EventTypeId
    INNER JOIN dbo.EventCategories ec ON ec.Id = e.EventCategoryId
    INNER JOIN dbo.EventStatuses es ON es.Id = e.EventStatusId
    WHERE e.Id = @Id;
END;
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_GetUpcoming
    @Now DATETIME2,
    @Limit INT,
    @EventTypeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Limit)
        e.Id,
        e.Title,
        e.Description,
        e.StartTime,
        e.EndTime,
        e.IsAllDay,
        e.Location,
        e.EventTypeId,
        et.Name AS EventTypeName,
        e.EventCategoryId,
        ec.Name AS EventCategoryName,
        e.EventStatusId,
        es.Name AS EventStatusName,
        e.ReminderMinutesBefore,
        e.CreatedAt,
        e.UpdatedAt
    FROM dbo.CalendarEvents e
    INNER JOIN dbo.EventTypes et ON et.Id = e.EventTypeId
    INNER JOIN dbo.EventCategories ec ON ec.Id = e.EventCategoryId
    INNER JOIN dbo.EventStatuses es ON es.Id = e.EventStatusId
    WHERE e.EndTime >= @Now
      AND (@EventTypeId IS NULL OR e.EventTypeId = @EventTypeId)
      AND (es.Name <> N'Cancelled')
    ORDER BY e.StartTime ASC;
END;
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_GetPast
    @Now DATETIME2,
    @Limit INT,
    @EventTypeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Limit)
        e.Id,
        e.Title,
        e.Description,
        e.StartTime,
        e.EndTime,
        e.IsAllDay,
        e.Location,
        e.EventTypeId,
        et.Name AS EventTypeName,
        e.EventCategoryId,
        ec.Name AS EventCategoryName,
        e.EventStatusId,
        es.Name AS EventStatusName,
        e.ReminderMinutesBefore,
        e.CreatedAt,
        e.UpdatedAt
    FROM dbo.CalendarEvents e
    INNER JOIN dbo.EventTypes et ON et.Id = e.EventTypeId
    INNER JOIN dbo.EventCategories ec ON ec.Id = e.EventCategoryId
    INNER JOIN dbo.EventStatuses es ON es.Id = e.EventStatusId
    WHERE e.EndTime < @Now
      AND (@EventTypeId IS NULL OR e.EventTypeId = @EventTypeId)
      AND (es.Name <> N'Cancelled')
    ORDER BY e.StartTime DESC;
END;
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_GetStatistics
    @Now DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Filtered AS
    (
        SELECT e.*
        FROM dbo.CalendarEvents e
        INNER JOIN dbo.EventStatuses es ON es.Id = e.EventStatusId
        WHERE es.Name <> N'Cancelled'
    )
    SELECT
        (SELECT COUNT(*) FROM dbo.CalendarEvents) AS TotalEvents,
        (SELECT COUNT(*) FROM Filtered WHERE EndTime >= @Now) AS UpcomingEvents,
        (SELECT COUNT(*) FROM Filtered WHERE StartTime >= @Now AND StartTime < DATEADD(DAY, 1, @Now)) AS TodaysEvents,
        (SELECT COUNT(*) FROM Filtered WHERE EndTime < @Now) AS CompletedEvents,
        (SELECT COUNT(*) FROM Filtered WHERE ReminderMinutesBefore IS NOT NULL AND EndTime >= @Now) AS RemindersSet,
        (SELECT COUNT(*) FROM Filtered WHERE EventTypeId = (SELECT Id FROM dbo.EventTypes WHERE Name = N'Meeting') AND EndTime >= @Now) AS UpcomingMeetings,
        (SELECT COUNT(*) FROM Filtered WHERE EventTypeId = (SELECT Id FROM dbo.EventTypes WHERE Name = N'Meeting') AND EndTime < @Now) AS CompletedMeetings;
END;
GO

CREATE PROCEDURE dbo.usp_CalendarEvent_SelectAll
    @FromDate DATETIME2 = NULL,
    @ToDate DATETIME2 = NULL,
    @EventTypeId INT = NULL,
    @EventCategoryId INT = NULL,
    @EventStatusId INT = NULL,
    @HasReminder BIT = NULL,
    @Scope INT = 0,
    @Now DATETIME2,
    @Search NVARCHAR(200) = NULL,
    @Page INT,
    @PageSize INT,
    @SortBy INT,
    @SortDirection INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MinRow INT = (@Page - 1) * @PageSize + 1;
    DECLARE @MaxRow INT = @Page * @PageSize;

    ;WITH Filtered AS
    (
        SELECT
            e.Id,
            e.Title,
            e.Description,
            e.StartTime,
            e.EndTime,
            e.IsAllDay,
            e.Location,
            e.EventTypeId,
            e.EventCategoryId,
            e.EventStatusId,
            et.Name AS EventTypeName,
            ec.Name AS EventCategoryName,
            es.Name AS EventStatusName,
            e.ReminderMinutesBefore,
            e.CreatedAt,
            e.UpdatedAt
        FROM dbo.CalendarEvents e
        INNER JOIN dbo.EventTypes et ON et.Id = e.EventTypeId
        INNER JOIN dbo.EventCategories ec ON ec.Id = e.EventCategoryId
        INNER JOIN dbo.EventStatuses es ON es.Id = e.EventStatusId
        WHERE e.EndTime >= @Now
          AND (@EventStatusId IS NULL OR e.EventStatusId = @EventStatusId)
          AND (@EventTypeId IS NULL OR e.EventTypeId = @EventTypeId)
          AND (@EventCategoryId IS NULL OR e.EventCategoryId = @EventCategoryId)
          AND (@HasReminder IS NULL OR e.ReminderMinutesBefore IS NOT NULL)
          AND (@Scope = 0 OR (@Scope = 1 AND e.EndTime >= @Now) OR (@Scope = 2 AND e.EndTime < @Now))
          AND (@FromDate IS NULL OR e.EndTime >= @FromDate)
          AND (@ToDate IS NULL OR e.StartTime <= @ToDate)
          AND (@Search IS NULL OR (e.Title LIKE '%' + @Search + '%'
                                    OR (e.Description IS NOT NULL AND e.Description LIKE '%' + @Search + '%')
                                    OR (e.Location IS NOT NULL AND e.Location LIKE '%' + @Search + '%')))
          AND (es.Name <> N'Cancelled')
    )
    SELECT *
    FROM
    (
        SELECT
            f.*,
            ROW_NUMBER() OVER (
                ORDER BY
                    CASE WHEN @SortBy = 1 AND @SortDirection = 1 THEN f.StartTime END DESC,
                    CASE WHEN @SortBy = 1 AND @SortDirection = 0 THEN f.StartTime END ASC,
                    CASE WHEN @SortBy = 2 AND @SortDirection = 1 THEN f.EndTime END DESC,
                    CASE WHEN @SortBy = 2 AND @SortDirection = 0 THEN f.EndTime END ASC,
                    CASE WHEN @SortBy = 3 AND @SortDirection = 1 THEN f.Title END DESC,
                    CASE WHEN @SortBy = 3 AND @SortDirection = 0 THEN f.Title END ASC,
                    CASE WHEN @SortBy = 4 AND @SortDirection = 1 THEN f.CreatedAt END DESC,
                    CASE WHEN @SortBy = 4 AND @SortDirection = 0 THEN f.CreatedAt END ASC
            ) AS RowNumber,
            COUNT(*) OVER () AS TotalCount
        FROM Filtered f
    ) AS numbered
    WHERE numbered.RowNumber BETWEEN @MinRow AND @MaxRow
    ORDER BY
        CASE WHEN @SortBy = 1 AND @SortDirection = 1 THEN numbered.StartTime END DESC,
        CASE WHEN @SortBy = 1 AND @SortDirection = 0 THEN numbered.StartTime END ASC,
        CASE WHEN @SortBy = 2 AND @SortDirection = 1 THEN numbered.EndTime END DESC,
        CASE WHEN @SortBy = 2 AND @SortDirection = 0 THEN numbered.EndTime END ASC,
        CASE WHEN @SortBy = 3 AND @SortDirection = 1 THEN numbered.Title END DESC,
        CASE WHEN @SortBy = 3 AND @SortDirection = 0 THEN numbered.Title END ASC,
        CASE WHEN @SortBy = 4 AND @SortDirection = 1 THEN numbered.CreatedAt END DESC,
        CASE WHEN @SortBy = 4 AND @SortDirection = 0 THEN numbered.CreatedAt END ASC;
END;
GO

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

CREATE PROCEDURE dbo.usp_Users_ChangePassword
    @UserId INT,
    @NewPasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET PasswordHash = @NewPasswordHash,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @UserId;
END;
GO

CREATE PROCEDURE dbo.usp_Users_GetSettings
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, Theme, NotificationsEnabled, EmailRemindersEnabled, UpdatedAt
    FROM dbo.UserSettings
    WHERE UserId = @UserId;
END;
GO

CREATE PROCEDURE dbo.usp_Users_UpdateSettings
    @UserId INT,
    @Theme NVARCHAR(20),
    @NotificationsEnabled BIT,
    @EmailRemindersEnabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.UserSettings WHERE UserId = @UserId)
    BEGIN
        UPDATE dbo.UserSettings
        SET Theme = @Theme,
            NotificationsEnabled = @NotificationsEnabled,
            EmailRemindersEnabled = @EmailRemindersEnabled,
            UpdatedAt = SYSUTCDATETIME()
        WHERE UserId = @UserId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.UserSettings (UserId, Theme, NotificationsEnabled, EmailRemindersEnabled, UpdatedAt)
        VALUES (@UserId, @Theme, @NotificationsEnabled, @EmailRemindersEnabled, SYSUTCDATETIME());
    END;
END;
GO

CREATE PROCEDURE dbo.usp_Users_Delete
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.UserSettings WHERE UserId = @UserId;
    DELETE FROM dbo.Users WHERE Id = @UserId;
END;
GO

IF OBJECT_ID(N'dbo.fn_CalendarEvent_IsOverlapping', N'FN') IS NOT NULL DROP FUNCTION dbo.fn_CalendarEvent_IsOverlapping;
GO

CREATE FUNCTION dbo.fn_CalendarEvent_IsOverlapping
(
    @StartTime DATETIME2,
    @EndTime DATETIME2,
    @CalendarEventId INT = NULL,
    @IsSelf BIT = 0
)
RETURNS BIT
AS
BEGIN
    DECLARE @Overlaps BIT = 0;

    IF @IsSelf = 0 AND @EndTime > @StartTime
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM dbo.CalendarEvents e
            INNER JOIN dbo.EventStatuses es ON es.Id = e.EventStatusId
            WHERE (@CalendarEventId IS NULL OR e.Id <> @CalendarEventId)
              AND e.StartTime < @EndTime
              AND e.EndTime > @StartTime
              AND es.Name <> N'Cancelled'
        )
        BEGIN
            SET @Overlaps = 1;
        END;
    END;

    RETURN @Overlaps;
END;
GO

CREATE VIEW dbo.vw_CalendarEvents_Details AS
SELECT
    e.Id,
    e.Title,
    e.Description,
    e.StartTime,
    e.EndTime,
    e.IsAllDay,
    e.Location,
    e.EventTypeId,
    et.Name AS EventTypeName,
    e.EventCategoryId,
    ec.Name AS EventCategoryName,
    e.EventStatusId,
    es.Name AS EventStatusName,
    e.ReminderMinutesBefore,
    e.CreatedAt,
    e.UpdatedAt
FROM dbo.CalendarEvents e
INNER JOIN dbo.EventTypes et ON et.Id = e.EventTypeId
INNER JOIN dbo.EventCategories ec ON ec.Id = e.EventCategoryId
INNER JOIN dbo.EventStatuses es ON es.Id = e.EventStatusId;
GO

SET IDENTITY_INSERT dbo.EventTypes ON;
INSERT INTO dbo.EventTypes (Id, Name) VALUES (1, N'Event'), (2, N'Meeting');
SET IDENTITY_INSERT dbo.EventTypes OFF;

SET IDENTITY_INSERT dbo.EventCategories ON;
INSERT INTO dbo.EventCategories (Id, Name, DisplayOrder) VALUES
    (1, N'Work', 1),
    (2, N'Personal', 2),
    (3, N'Birthday', 3),
    (4, N'Holiday', 4),
    (5, N'Other', 5);
SET IDENTITY_INSERT dbo.EventCategories OFF;

SET IDENTITY_INSERT dbo.EventStatuses ON;
INSERT INTO dbo.EventStatuses (Id, Name) VALUES
    (1, N'Confirmed'),
    (2, N'Tentative'),
    (3, N'Cancelled');
SET IDENTITY_INSERT dbo.EventStatuses OFF;
GO
