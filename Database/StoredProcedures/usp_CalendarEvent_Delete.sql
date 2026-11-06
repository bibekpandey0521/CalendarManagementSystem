CREATE PROCEDURE dbo.usp_CalendarEvent_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.CalendarEvents
    WHERE Id = @Id;
END;
GO
