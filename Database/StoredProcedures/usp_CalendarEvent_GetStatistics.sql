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
