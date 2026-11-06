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
