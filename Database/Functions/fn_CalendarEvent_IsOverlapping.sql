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
