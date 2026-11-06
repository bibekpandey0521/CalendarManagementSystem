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
