-- Script executed against the live SQL Server instance before the API starts.

IF OBJECT_ID(N'dbo.usp_CalendarEvent_SelectAll', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_SelectAll;
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetUpcoming', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetUpcoming;
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetPast', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetPast;
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetStatistics', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetStatistics;
IF OBJECT_ID(N'dbo.usp_CalendarEvent_GetById', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_GetById;
IF OBJECT_ID(N'dbo.usp_CalendarEvent_Update', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_Update;
IF OBJECT_ID(N'dbo.usp_CalendarEvent_Delete', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_Delete;
IF OBJECT_ID(N'dbo.usp_CalendarEvent_Create', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalendarEvent_Create;
IF OBJECT_ID(N'dbo.fn_CalendarEvent_IsOverlapping', N'F') IS NOT NULL DROP FUNCTION dbo.fn_CalendarEvent_IsOverlapping;
IF OBJECT_ID(N'dbo.vw_CalendarEvents_Details', N'V') IS NOT NULL DROP VIEW dbo.vw_CalendarEvents_Details;
IF OBJECT_ID(N'dbo.CalendarEvents', N'U') IS NOT NULL DROP TABLE dbo.CalendarEvents;
IF OBJECT_ID(N'dbo.EventStatuses', N'U') IS NOT NULL DROP TABLE dbo.EventStatuses;
IF OBJECT_ID(N'dbo.EventCategories', N'U') IS NOT NULL DROP TABLE dbo.EventCategories;
IF OBJECT_ID(N'dbo.EventTypes', N'U') IS NOT NULL DROP TABLE dbo.EventTypes;
GO
