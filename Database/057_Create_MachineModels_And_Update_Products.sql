-- Migration 057: Create Machine Models Master and Update Products
-- Purpose: Add Machine Models master table and link Products to Models via FK

USE MultiHitechERP;
GO

PRINT 'Starting migration 057: Create MachineModels and update Products...';
GO

-- Step 1: Create MachineModels table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_MachineModels')
BEGIN
    PRINT 'Creating Masters_MachineModels table...';

    CREATE TABLE Masters_MachineModels (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ModelName NVARCHAR(200) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'System',
        UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
        IsActive BIT NOT NULL DEFAULT 1
    );

    -- Create unique index on ModelName to prevent duplicates
    CREATE UNIQUE INDEX IX_MachineModels_ModelName ON Masters_MachineModels (ModelName);

    PRINT 'Masters_MachineModels table created.';
END
ELSE
BEGIN
    PRINT 'Masters_MachineModels table already exists.';
END
GO

-- Step 2: Migrate existing ModelName data to MachineModels table
PRINT 'Migrating existing model names from Products...';

INSERT INTO Masters_MachineModels (ModelName, CreatedBy, CreatedAt, UpdatedAt)
SELECT DISTINCT
    ModelName,
    'Migration',
    GETUTCDATE(),
    GETUTCDATE()
FROM Masters_Products
WHERE ModelName IS NOT NULL
  AND ModelName <> ''
  AND ModelName NOT IN (SELECT ModelName FROM Masters_MachineModels);

PRINT CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' machine models migrated.';
GO

-- Step 3: Add ModelId column to Products table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'ModelId')
BEGIN
    PRINT 'Adding ModelId column to Masters_Products...';

    ALTER TABLE Masters_Products
    ADD ModelId INT NULL;

    PRINT 'ModelId column added.';
END
ELSE
BEGIN
    PRINT 'ModelId column already exists.';
END
GO

-- Step 4: Populate ModelId based on existing ModelName
PRINT 'Populating ModelId from ModelName...';

UPDATE p
SET p.ModelId = m.Id
FROM Masters_Products p
INNER JOIN Masters_MachineModels m ON p.ModelName = m.ModelName
WHERE p.ModelName IS NOT NULL AND p.ModelName <> '';

PRINT CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' products updated with ModelId.';
GO

-- Step 5: Make ModelId required (NOT NULL) and add foreign key
-- First, set a default ModelId for any products without one
DECLARE @DefaultModelId INT;

-- Create a default "Unknown Model" if needed
IF NOT EXISTS (SELECT * FROM Masters_MachineModels WHERE ModelName = 'Unknown Model')
BEGIN
    INSERT INTO Masters_MachineModels (ModelName, CreatedBy, CreatedAt, UpdatedAt)
    VALUES ('Unknown Model', 'System', GETUTCDATE(), GETUTCDATE());
END

SELECT @DefaultModelId = Id FROM Masters_MachineModels WHERE ModelName = 'Unknown Model';

-- Set default ModelId for products that don't have one
UPDATE Masters_Products
SET ModelId = @DefaultModelId
WHERE ModelId IS NULL;

-- Now make ModelId NOT NULL
ALTER TABLE Masters_Products
ALTER COLUMN ModelId INT NOT NULL;

PRINT 'ModelId column set to NOT NULL.';
GO

-- Step 6: Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_MachineModels')
BEGIN
    PRINT 'Adding foreign key constraint...';

    ALTER TABLE Masters_Products
    ADD CONSTRAINT FK_Products_MachineModels
    FOREIGN KEY (ModelId) REFERENCES Masters_MachineModels(Id);

    PRINT 'Foreign key constraint added.';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint already exists.';
END
GO

-- Step 7: Create index on ModelId for performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_ModelId')
BEGIN
    CREATE INDEX IX_Products_ModelId ON Masters_Products (ModelId);
    PRINT 'Index IX_Products_ModelId created.';
END
GO

-- Step 8: Optional - Drop old ModelName column (commented out for safety)
-- Uncomment after verifying everything works
/*
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Products') AND name = 'ModelName')
BEGIN
    PRINT 'Dropping old ModelName column...';
    ALTER TABLE Masters_Products DROP COLUMN ModelName;
    PRINT 'ModelName column dropped.';
END
*/

PRINT '';
PRINT '=== Migration Summary ===';
PRINT 'MachineModels table:';
SELECT COUNT(*) AS TotalModels FROM Masters_MachineModels;

PRINT '';
PRINT 'Products with ModelId:';
SELECT COUNT(*) AS ProductsWithModel FROM Masters_Products WHERE ModelId IS NOT NULL;

PRINT '';
PRINT 'Products by Model:';
SELECT m.ModelName, COUNT(p.Id) AS ProductCount
FROM Masters_MachineModels m
LEFT JOIN Masters_Products p ON m.Id = p.ModelId
GROUP BY m.ModelName
ORDER BY ProductCount DESC;

PRINT '';
PRINT 'Migration 057 completed successfully!';
PRINT 'NOTE: Old ModelName column is kept for backward compatibility. Remove manually after verification.';
GO
