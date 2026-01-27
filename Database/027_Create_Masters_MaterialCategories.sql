-- Migration: Create Masters_MaterialCategories table
-- Description: Categories for organizing materials (raw materials and components)
-- Note: This is master data for categorization purposes

-- Drop table if exists (for clean migration during development)
IF OBJECT_ID('Masters_MaterialCategories', 'U') IS NOT NULL
BEGIN
    DROP TABLE Masters_MaterialCategories;
    PRINT 'Existing Masters_MaterialCategories table dropped';
END
GO

-- Create Masters_MaterialCategories table
CREATE TABLE Masters_MaterialCategories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryCode NVARCHAR(50) NOT NULL UNIQUE,
    CategoryName NVARCHAR(200) NOT NULL,
    Quality NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    DefaultUOM NVARCHAR(20) NOT NULL,
    MaterialType NVARCHAR(20) NOT NULL CHECK (MaterialType IN ('raw_material', 'component')),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Create index on CategoryCode for faster lookups
CREATE INDEX IX_MaterialCategories_CategoryCode ON Masters_MaterialCategories(CategoryCode);
GO

-- Create index on MaterialType for filtering
CREATE INDEX IX_MaterialCategories_MaterialType ON Masters_MaterialCategories(MaterialType);
GO

-- Create index on IsActive for filtering
CREATE INDEX IX_MaterialCategories_IsActive ON Masters_MaterialCategories(IsActive);
GO

PRINT 'Masters_MaterialCategories table created successfully';
GO
