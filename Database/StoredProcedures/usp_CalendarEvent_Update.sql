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
