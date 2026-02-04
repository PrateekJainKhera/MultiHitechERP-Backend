-- =============================================
-- Migration: Update Masters_Drawings Table
-- Description: Update to match frontend requirements with file storage and linking
-- Date: 2024-02-04
-- =============================================

USE MultiHitechERP
GO

PRINT 'Starting Masters_Drawings migration...';
PRINT '';

-- Step 1: Drop foreign keys from OTHER tables that reference Masters_Drawings
PRINT 'Step 1: Dropping foreign keys from Orders table...';
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Drawing')
BEGIN
    ALTER TABLE Orders DROP CONSTRAINT FK_Orders_Drawing;
    PRINT '  ✓ Dropped FK_Orders_Drawing';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_PrimaryDrawing')
BEGIN
    ALTER TABLE Orders DROP CONSTRAINT FK_Orders_PrimaryDrawing;
    PRINT '  ✓ Dropped FK_Orders_PrimaryDrawing';
END

GO

-- Step 2: Drop existing constraints on Masters_Drawings itself
PRINT 'Step 2: Dropping constraints on Masters_Drawings table...';
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Drawings_Customer')
BEGIN
    ALTER TABLE Masters_Drawings DROP CONSTRAINT FK_Drawings_Customer;
    PRINT '  ✓ Dropped FK_Drawings_Customer';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Drawings_Product')
BEGIN
    ALTER TABLE Masters_Drawings DROP CONSTRAINT FK_Drawings_Product;
    PRINT '  ✓ Dropped FK_Drawings_Product';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Drawings_Part')
BEGIN
    ALTER TABLE Masters_Drawings DROP CONSTRAINT FK_Drawings_Part;
    PRINT '  ✓ Dropped FK_Drawings_Part';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_Masters_Drawings_DrawingNumber' AND object_id = OBJECT_ID('Masters_Drawings'))
BEGIN
    ALTER TABLE Masters_Drawings DROP CONSTRAINT UQ_Masters_Drawings_DrawingNumber;
    PRINT '  ✓ Dropped UQ_Masters_Drawings_DrawingNumber';
END

GO

-- Step 3: Backup existing data
PRINT 'Step 3: Backing up existing data...';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Masters_Drawings_Backup')
    DROP TABLE Masters_Drawings_Backup;

SELECT * INTO Masters_Drawings_Backup FROM Masters_Drawings;
DECLARE @BackupCount INT = (SELECT COUNT(*) FROM Masters_Drawings_Backup);
PRINT '  ✓ Backed up ' + CAST(@BackupCount AS NVARCHAR(10)) + ' rows to Masters_Drawings_Backup';

GO

-- Step 4: Drop and recreate table
PRINT 'Step 4: Recreating Masters_Drawings table with new structure...';
DROP TABLE Masters_Drawings;

CREATE TABLE Masters_Drawings (
    Id INT PRIMARY KEY IDENTITY(1,1),

    -- Basic Information
    DrawingNumber NVARCHAR(100) NOT NULL,
    DrawingName NVARCHAR(200) NOT NULL,
    PartType NVARCHAR(50) NOT NULL, -- shaft, pipe, final, gear, bushing, roller, other
    Revision NVARCHAR(20) NULL,
    RevisionDate DATE NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'draft', -- draft, approved, obsolete

    -- File Information
    FileName NVARCHAR(255) NULL,
    FileType NVARCHAR(50) NULL, -- pdf, image, dwg
    FileUrl NVARCHAR(500) NULL,
    FileSize DECIMAL(10,2) NULL, -- in KB

    -- Manufacturing Dimensions (stored as JSON for flexibility)
    ManufacturingDimensionsJSON NVARCHAR(MAX) NULL,

    -- Linking to other entities
    LinkedPartId INT NULL,
    LinkedProductId INT NULL,
    LinkedCustomerId INT NULL,

    -- Metadata
    Description NVARCHAR(500) NULL,
    Notes NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    -- Audit fields
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL,
    ApprovedBy NVARCHAR(100) NULL,
    ApprovedAt DATETIME NULL,

    -- Constraints
    CONSTRAINT UQ_Masters_Drawings_DrawingNumber UNIQUE (DrawingNumber)
);

PRINT '  ✓ Masters_Drawings table recreated';

GO

-- Step 5: Migrate existing data from backup
PRINT 'Step 5: Migrating existing data...';
SET IDENTITY_INSERT Masters_Drawings ON;

INSERT INTO Masters_Drawings (
    Id, DrawingNumber, DrawingName, PartType, Revision, Status,
    Description, Notes, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
)
SELECT
    Id,
    DrawingNumber,
    DrawingName,
    COALESCE(DrawingType, 'other') as PartType, -- Map old DrawingType to new PartType
    COALESCE(RevisionNumber, 'A') as Revision, -- Map old RevisionNumber to new Revision
    COALESCE(Status, 'draft') as Status,
    Description,
    Remarks as Notes, -- Map old Remarks to new Notes
    IsActive,
    CreatedAt,
    CreatedBy,
    UpdatedAt,
    UpdatedBy
FROM Masters_Drawings_Backup;

SET IDENTITY_INSERT Masters_Drawings OFF;

DECLARE @MigratedCount INT = (SELECT COUNT(*) FROM Masters_Drawings);
PRINT '  ✓ Migrated ' + CAST(@MigratedCount AS NVARCHAR(10)) + ' rows';

GO

-- Step 6: Add Foreign Key Constraints to Masters_Drawings
PRINT 'Step 6: Adding foreign key constraints to Masters_Drawings...';
ALTER TABLE Masters_Drawings
ADD CONSTRAINT FK_Drawings_Part
FOREIGN KEY (LinkedPartId) REFERENCES Masters_RawMaterials(Id);
PRINT '  ✓ Added FK_Drawings_Part';

ALTER TABLE Masters_Drawings
ADD CONSTRAINT FK_Drawings_Product
FOREIGN KEY (LinkedProductId) REFERENCES Masters_Products(Id);
PRINT '  ✓ Added FK_Drawings_Product';

ALTER TABLE Masters_Drawings
ADD CONSTRAINT FK_Drawings_Customer
FOREIGN KEY (LinkedCustomerId) REFERENCES Masters_Customers(Id);
PRINT '  ✓ Added FK_Drawings_Customer';

GO

-- Step 7: Recreate foreign keys FROM Orders table TO Masters_Drawings
PRINT 'Step 7: Recreating foreign keys from Orders table...';
-- Note: Only recreate if the columns exist in Orders table
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'DrawingId')
BEGIN
    ALTER TABLE Orders
    ADD CONSTRAINT FK_Orders_Drawing
    FOREIGN KEY (DrawingId) REFERENCES Masters_Drawings(Id);
    PRINT '  ✓ Recreated FK_Orders_Drawing';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'PrimaryDrawingId')
BEGIN
    ALTER TABLE Orders
    ADD CONSTRAINT FK_Orders_PrimaryDrawing
    FOREIGN KEY (PrimaryDrawingId) REFERENCES Masters_Drawings(Id);
    PRINT '  ✓ Recreated FK_Orders_PrimaryDrawing';
END

GO

-- Step 8: Create indexes for better performance
PRINT 'Step 8: Creating indexes...';
CREATE NONCLUSTERED INDEX IX_Drawings_Status ON Masters_Drawings(Status);
CREATE NONCLUSTERED INDEX IX_Drawings_PartType ON Masters_Drawings(PartType);
CREATE NONCLUSTERED INDEX IX_Drawings_LinkedProduct ON Masters_Drawings(LinkedProductId);
CREATE NONCLUSTERED INDEX IX_Drawings_LinkedCustomer ON Masters_Drawings(LinkedCustomerId);
CREATE NONCLUSTERED INDEX IX_Drawings_CreatedAt ON Masters_Drawings(CreatedAt DESC);
PRINT '  ✓ Created 5 indexes';

GO

-- Step 9: Create file storage directory path helper
PRINT 'Step 9: Creating stored procedure...';
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GenerateDrawingFilePath')
    DROP PROCEDURE sp_GenerateDrawingFilePath;

CREATE PROCEDURE sp_GenerateDrawingFilePath
    @DrawingNumber NVARCHAR(100),
    @FileName NVARCHAR(255),
    @FilePath NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Year NVARCHAR(4) = CAST(YEAR(GETDATE()) AS NVARCHAR(4));
    DECLARE @Month NVARCHAR(2) = RIGHT('0' + CAST(MONTH(GETDATE()) AS NVARCHAR(2)), 2);

    -- Format: /uploads/drawings/2024/02/DWG-001_filename.pdf
    SET @FilePath = '/uploads/drawings/' + @Year + '/' + @Month + '/' + @DrawingNumber + '_' + @FileName;
END;

PRINT '  ✓ Created sp_GenerateDrawingFilePath';

GO

-- Step 10: Insert sample data for testing
PRINT 'Step 10: Inserting sample data...';
IF NOT EXISTS (SELECT * FROM Masters_Drawings WHERE DrawingNumber = 'DWG-SAMPLE-001')
BEGIN
    INSERT INTO Masters_Drawings (
        DrawingNumber, DrawingName, PartType, Revision, RevisionDate, Status,
        FileName, FileType, FileUrl, FileSize,
        Description, Notes, CreatedBy, CreatedAt
    ) VALUES
    (
        'DWG-SAMPLE-001',
        'Sample Main Shaft Assembly',
        'shaft',
        'A',
        '2024-02-04',
        'draft',
        'shaft-main-assembly.pdf',
        'pdf',
        '/uploads/drawings/2024/02/DWG-SAMPLE-001_shaft-main-assembly.pdf',
        245.5,
        'Sample drawing for testing - Main shaft for printing roller assembly',
        'Critical tolerance: ±0.01mm on bearing surfaces',
        'System',
        GETDATE()
    );
    PRINT '  ✓ Inserted sample drawing DWG-SAMPLE-001';
END

GO

-- Step 11: Verify the new structure
PRINT '';
PRINT 'Verification - New table structure:';
PRINT '=====================================';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Drawings'
ORDER BY ORDINAL_POSITION;

GO

PRINT '';
PRINT '✓✓✓ Masters_Drawings migration completed successfully! ✓✓✓';
PRINT '';
PRINT 'Summary:';
PRINT '  - Table structure updated with all required fields';
PRINT '  - Existing data preserved and migrated';
PRINT '  - Foreign key constraints added';
PRINT '  - Indexes created for performance';
PRINT '  - Sample data inserted';
PRINT '';
PRINT 'Next steps:';
PRINT '  1. Create uploads/drawings folder structure in backend';
PRINT '  2. Update backend C# DTOs and repository';
PRINT '  3. Implement file upload endpoints';
PRINT '';

GO
