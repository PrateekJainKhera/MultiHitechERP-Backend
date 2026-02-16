-- Migration 066: Create Process Category Master
-- Date: 2026-02-16
-- Description: Creates Process Category Master table for grouping processes (Turning 1, Turning 2, Grinding, etc.)

-- Create Process Category Master table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_ProcessCategories')
BEGIN
    CREATE TABLE Masters_ProcessCategories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CategoryCode NVARCHAR(50) NOT NULL UNIQUE,
        CategoryName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,

        -- Metadata
        IsActive BIT NOT NULL DEFAULT 1,

        -- Audit fields
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(100) NULL,
        UpdatedAt DATETIME NULL,
        UpdatedBy NVARCHAR(100) NULL,

        -- Indexes
        INDEX IX_ProcessCategories_CategoryCode (CategoryCode),
        INDEX IX_ProcessCategories_IsActive (IsActive)
    );

    PRINT 'Created table: Masters_ProcessCategories';
END
ELSE
BEGIN
    PRINT 'Table Masters_ProcessCategories already exists';
END
GO

-- Insert default process categories
IF NOT EXISTS (SELECT 1 FROM Masters_ProcessCategories WHERE CategoryCode = 'TURN-1')
BEGIN
    INSERT INTO Masters_ProcessCategories (CategoryCode, CategoryName, Description, IsActive)
    VALUES
        ('TURN-1', 'Turning 1', 'Basic turning operations on CNC lathes', 1),
        ('TURN-2', 'Turning 2', 'Advanced turning operations with complex geometries', 1),
        ('GRIND-1', 'Grinding 1', 'Cylindrical grinding operations', 1),
        ('GRIND-2', 'Grinding 2', 'Surface and precision grinding', 1),
        ('MILL-1', 'Milling 1', 'Basic milling operations', 1),
        ('MILL-2', 'Milling 2', 'CNC milling with multi-axis capability', 1),
        ('DRILL', 'Drilling', 'Drilling and boring operations', 1),
        ('WELD', 'Welding', 'Welding and fabrication', 1),
        ('ASSY', 'Assembly', 'Assembly operations', 1),
        ('QC', 'Quality Check', 'Quality inspection and testing', 1);

    PRINT 'Inserted default process categories';
END
GO
