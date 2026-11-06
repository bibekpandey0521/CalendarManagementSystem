CREATE TABLE dbo.EventCategories
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EventCategories PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL CONSTRAINT UQ_EventCategories_Name UNIQUE,
    DisplayOrder INT NOT NULL CONSTRAINT DF_EventCategories_DisplayOrder DEFAULT (0)
);
GO
