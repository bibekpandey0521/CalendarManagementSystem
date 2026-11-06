SET IDENTITY_INSERT dbo.EventTypes ON;
INSERT INTO dbo.EventTypes (Id, Name) VALUES (1, N'Event'), (2, N'Meeting');
SET IDENTITY_INSERT dbo.EventTypes OFF;

SET IDENTITY_INSERT dbo.EventCategories ON;
INSERT INTO dbo.EventCategories (Id, Name, DisplayOrder) VALUES
    (1, N'Work', 1),
    (2, N'Personal', 2),
    (3, N'Birthday', 3),
    (4, N'Holiday', 4),
    (5, N'Other', 5);
SET IDENTITY_INSERT dbo.EventCategories OFF;

SET IDENTITY_INSERT dbo.EventStatuses ON;
INSERT INTO dbo.EventStatuses (Id, Name) VALUES
    (1, N'Confirmed'),
    (2, N'Tentative'),
    (3, N'Cancelled');
SET IDENTITY_INSERT dbo.EventStatuses OFF;
GO