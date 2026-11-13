CREATE PROCEDURE dbo.usp_MeetingParticipants_GetByMeetingId
    @MeetingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, MeetingId, UserId, ParticipantName, Email, CreatedAt
    FROM dbo.MeetingParticipants
    WHERE MeetingId = @MeetingId
    ORDER BY CreatedAt ASC;
END;
GO
