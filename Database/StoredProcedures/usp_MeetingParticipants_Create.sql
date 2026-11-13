CREATE PROCEDURE dbo.usp_MeetingParticipants_Create
    @MeetingId INT,
    @UserId INT = NULL,
    @ParticipantName NVARCHAR(200) = NULL,
    @Email NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for duplicate participant
    IF EXISTS (SELECT 1 FROM dbo.MeetingParticipants WHERE MeetingId = @MeetingId AND Email = @Email)
    BEGIN
        RETURN;
    END;

    INSERT INTO dbo.MeetingParticipants (MeetingId, UserId, ParticipantName, Email, CreatedAt)
    VALUES (@MeetingId, @UserId, @ParticipantName, @Email, SYSUTCDATETIME());

    SELECT SCOPE_IDENTITY() AS ParticipantId;
END;
GO
