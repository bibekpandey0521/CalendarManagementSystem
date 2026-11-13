CREATE PROCEDURE dbo.usp_MeetingParticipants_Delete
    @MeetingId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.MeetingParticipants WHERE MeetingId = @MeetingId;
END;
GO
