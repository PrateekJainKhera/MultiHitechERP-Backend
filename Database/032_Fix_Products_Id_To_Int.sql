-- Migration 032: Change Masters_Products Id from UNIQUEIDENTIFIER to INT IDENTITY
-- Purpose: Standardize all IDs to INT instead of UUID

USE MultiHitechERP;
GO

PRINT 'Starting migration 032: Fix Products Id to INT...';
GO

-- Step 1: Find and drop all foreign keys referencing Masters_Products
DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' +
               QUOTENAME(OBJECT_NAME(parent_object_id)) +
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('Masters_Products');

IF @sql <> ''
BEGIN
    PRINT 'Dropping foreign keys referencing Masters_Products...';
    EXEC sp_executesql @sql;
    PRINT 'Foreign keys dropped.';
END
GO

-- Step 2: Create new table with INT primary key
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_Products_New')
BEGIN
    PRINT 'Creating new Masters_Products table with INT Id...';

    CREATE TABLE Masters_Products_New (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PartCode NVARCHAR(50) NOT NULL,
        CustomerName NVARCHAR(200),
        ModelName NVARCHAR(200) NOT NULL,
        RollerType NVARCHAR(50) NOT NULL,
        Diameter DECIMAL(18,2) NOT NULL,
        Length DECIMAL(18,2) NOT NULL,
        MaterialGrade NVARCHAR(50),
        SurfaceFinish NVARCHAR(100),
        Hardness NVARCHAR(50),
        DrawingNo NVARCHAR(100),
        RevisionNo NVARCHAR(20),
        RevisionDate NVARCHAR(50),
        NumberOfTeeth INT NULL,
        ProcessTemplateId INT NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(100) NOT NULL,
        UpdatedAt DATETIME NOT NULL,
    );

    PRINT 'New table created.';
END
GO

-- Step 3: Copy data from old table to new table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_Products')
BEGIN
    PRINT 'Migrating data from old to new table...';

    SET IDENTITY_INSERT Masters_Products_New ON;

    INSERT INTO Masters_Products_New (
        Id, PartCode, CustomerName, ModelName, RollerType,
        Diameter, Length, MaterialGrade, SurfaceFinish, Hardness,
        DrawingNo, RevisionNo, RevisionDate, NumberOfTeeth,
        ProcessTemplateId, CreatedAt, CreatedBy, UpdatedAt
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY CreatedAt) as Id,  -- Assign new sequential IDs
        PartCode, CustomerName, ModelName, RollerType,
        Diameter, Length, MaterialGrade, SurfaceFinish, Hardness,
        DrawingNo, RevisionNo, RevisionDate, NumberOfTeeth,
        ProcessTemplateId, CreatedAt, CreatedBy, UpdatedAt
    FROM Masters_Products;

    SET IDENTITY_INSERT Masters_Products_New OFF;

    PRINT 'Data migrated.';
END
GO

-- Step 4: Drop old table and rename new one
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_Products')
BEGIN
    PRINT 'Dropping old Masters_Products table...';
    DROP TABLE Masters_Products;
    PRINT 'Old table dropped.';
END
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_Products_New')
BEGIN
    PRINT 'Renaming new table to Masters_Products...';
    EXEC sp_rename 'Masters_Products_New', 'Masters_Products';
    PRINT 'Table renamed.';
END
GO

-- Step 5: Recreate indexes
PRINT 'Creating indexes...';
CREATE INDEX IX_Products_PartCode ON Masters_Products (PartCode);
CREATE INDEX IX_Products_ProcessTemplateId ON Masters_Products (ProcessTemplateId);
GO

PRINT '';
PRINT '=== Final Products structure ===';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Products'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'Migration 032 completed: Products Id is now INT IDENTITY!';
GO
