SET QUOTED_IDENTIFIER ON;
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