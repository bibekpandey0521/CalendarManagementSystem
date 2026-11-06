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
