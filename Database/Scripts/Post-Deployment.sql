-- Seed EventTypes if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.EventTypes WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT dbo.EventTypes ON;
    INSERT INTO dbo.EventTypes (Id, Name) VALUES (1, N'Event'), (2, N'Meeting');
    SET IDENTITY_INSERT dbo.EventTypes OFF;
END;

-- Seed EventCategories if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.EventCategories WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT dbo.EventCategories ON;
    INSERT INTO dbo.EventCategories (Id, Name, DisplayOrder) VALUES
        (1, N'Work', 1),
        (2, N'Personal', 2),
        (3, N'Birthday', 3),
        (4, N'Holiday', 4),
        (5, N'Other', 5);
    SET IDENTITY_INSERT dbo.EventCategories OFF;
END;

-- Seed EventStatuses if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.EventStatuses WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT dbo.EventStatuses ON;
    INSERT INTO dbo.EventStatuses (Id, Name) VALUES
        (1, N'Confirmed'),
        (2, N'Tentative'),
        (3, N'Cancelled');
    SET IDENTITY_INSERT dbo.EventStatuses OFF;
END;

-- Seed Users if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT dbo.Users ON;
    INSERT INTO dbo.Users (Id, Email, FirstName, LastName, PasswordHash, IsEmailVerified, IsActive) VALUES
        (1, N'user@example.com', N'John', N'Doe', N'$2a$11$hashedpasswordplaceholder', 1, 1);
    SET IDENTITY_INSERT dbo.Users OFF;
END;

-- Seed UserSettings if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.UserSettings WHERE UserId = 1)
BEGIN
    SET IDENTITY_INSERT dbo.UserSettings ON;
    INSERT INTO dbo.UserSettings (Id, UserId, Theme, NotificationsEnabled, EmailRemindersEnabled) VALUES
        (1, 1, N'light', 1, 1);
    SET IDENTITY_INSERT dbo.UserSettings OFF;
END;
GO
