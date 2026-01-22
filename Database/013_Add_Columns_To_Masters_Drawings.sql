-- Add missing columns to Masters_Drawings table
USE MultiHitechERP;
GO

-- Verify table exists
IF OBJECT_ID('Masters_Drawings', 'U') IS NULL
BEGIN
    PRINT 'ERROR: Masters_Drawings table does not exist!';
    RETURN;
END

PRINT 'Adding missing columns to Masters_Drawings table...';
GO

-- Add DrawingTitle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'DrawingTitle')
BEGIN
    ALTER TABLE Masters_Drawings ADD DrawingTitle NVARCHAR(300) NULL;
    -- Copy from DrawingName if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'DrawingName')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET DrawingTitle = DrawingName';
    END
    PRINT 'Added DrawingTitle column';
END

-- Add ProductId (INT instead of GUID)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'ProductId')
BEGIN
    ALTER TABLE Masters_Drawings ADD ProductId INT NULL;
    PRINT 'Added ProductId column';
END

-- Add ProductCode
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'ProductCode')
BEGIN
    ALTER TABLE Masters_Drawings ADD ProductCode NVARCHAR(50) NULL;
    PRINT 'Added ProductCode column';
END

-- Add ProductName
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'ProductName')
BEGIN
    ALTER TABLE Masters_Drawings ADD ProductName NVARCHAR(200) NULL;
    PRINT 'Added ProductName column';
END

-- Add RevisionNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'RevisionNumber')
BEGIN
    ALTER TABLE Masters_Drawings ADD RevisionNumber NVARCHAR(20) NULL;
    -- Copy from Revision if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'Revision')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET RevisionNumber = Revision';
    END
    PRINT 'Added RevisionNumber column';
END

-- Add RevisionDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'RevisionDate')
BEGIN
    ALTER TABLE Masters_Drawings ADD RevisionDate DATETIME NULL;
    PRINT 'Added RevisionDate column';
END

-- Add RevisionDescription
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'RevisionDescription')
BEGIN
    ALTER TABLE Masters_Drawings ADD RevisionDescription NVARCHAR(MAX) NULL;
    PRINT 'Added RevisionDescription column';
END

-- Add DrawingType
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'DrawingType')
BEGIN
    ALTER TABLE Masters_Drawings ADD DrawingType NVARCHAR(100) NULL;
    -- Copy from PartType if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'PartType')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET DrawingType = PartType';
    END
    PRINT 'Added DrawingType column';
END

-- Add Category
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'Category')
BEGIN
    ALTER TABLE Masters_Drawings ADD Category NVARCHAR(100) NULL;
    PRINT 'Added Category column';
END

-- Add FilePath
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'FilePath')
BEGIN
    ALTER TABLE Masters_Drawings ADD FilePath NVARCHAR(500) NULL;
    -- Copy from FileStoragePath if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'FileStoragePath')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET FilePath = FileStoragePath';
    END
    PRINT 'Added FilePath column';
END

-- Add FileFormat
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'FileFormat')
BEGIN
    ALTER TABLE Masters_Drawings ADD FileFormat NVARCHAR(50) NULL;
    -- Copy from FileExtension if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'FileExtension')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET FileFormat = FileExtension';
    END
    PRINT 'Added FileFormat column';
END

-- Add FileSize (rename from FileSizeKB)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'FileSize')
BEGIN
    ALTER TABLE Masters_Drawings ADD FileSize BIGINT NULL;
    -- Copy from FileSizeKB if exists (convert KB to bytes)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'FileSizeKB')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET FileSize = FileSizeKB * 1024';
    END
    PRINT 'Added FileSize column';
END

-- Add PreparedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'PreparedBy')
BEGIN
    ALTER TABLE Masters_Drawings ADD PreparedBy NVARCHAR(100) NULL;
    -- Copy from UploadedBy if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'UploadedBy')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET PreparedBy = UploadedBy';
    END
    PRINT 'Added PreparedBy column';
END

-- Add CheckedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'CheckedBy')
BEGIN
    ALTER TABLE Masters_Drawings ADD CheckedBy NVARCHAR(100) NULL;
    PRINT 'Added CheckedBy column';
END

-- Add ApprovalDate (different from ApprovedDate)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'ApprovalDate')
BEGIN
    ALTER TABLE Masters_Drawings ADD ApprovalDate DATETIME NULL;
    -- Copy from ApprovedDate if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'ApprovedDate')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET ApprovalDate = ApprovedDate';
    END
    PRINT 'Added ApprovalDate column';
END

-- Add MaterialSpecification
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'MaterialSpecification')
BEGIN
    ALTER TABLE Masters_Drawings ADD MaterialSpecification NVARCHAR(200) NULL;
    PRINT 'Added MaterialSpecification column';
END

-- Add Finish
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'Finish')
BEGIN
    ALTER TABLE Masters_Drawings ADD Finish NVARCHAR(100) NULL;
    PRINT 'Added Finish column';
END

-- Add ToleranceGrade
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'ToleranceGrade')
BEGIN
    ALTER TABLE Masters_Drawings ADD ToleranceGrade NVARCHAR(50) NULL;
    PRINT 'Added ToleranceGrade column';
END

-- Add TreatmentRequired
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'TreatmentRequired')
BEGIN
    ALTER TABLE Masters_Drawings ADD TreatmentRequired NVARCHAR(200) NULL;
    PRINT 'Added TreatmentRequired column';
END

-- Add OverallLength
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'OverallLength')
BEGIN
    ALTER TABLE Masters_Drawings ADD OverallLength DECIMAL(18,3) NULL;
    PRINT 'Added OverallLength column';
END

-- Add OverallWidth
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'OverallWidth')
BEGIN
    ALTER TABLE Masters_Drawings ADD OverallWidth DECIMAL(18,3) NULL;
    PRINT 'Added OverallWidth column';
END

-- Add OverallHeight
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'OverallHeight')
BEGIN
    ALTER TABLE Masters_Drawings ADD OverallHeight DECIMAL(18,3) NULL;
    PRINT 'Added OverallHeight column';
END

-- Add Weight
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'Weight')
BEGIN
    ALTER TABLE Masters_Drawings ADD Weight DECIMAL(18,3) NULL;
    PRINT 'Added Weight column';
END

-- Add IsLatestRevision
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'IsLatestRevision')
BEGIN
    ALTER TABLE Masters_Drawings ADD IsLatestRevision BIT NOT NULL DEFAULT 1;
    PRINT 'Added IsLatestRevision column';
END

-- Add PreviousRevisionId
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'PreviousRevisionId')
BEGIN
    ALTER TABLE Masters_Drawings ADD PreviousRevisionId INT NULL;
    PRINT 'Added PreviousRevisionId column';
END

-- Add VersionNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'VersionNumber')
BEGIN
    ALTER TABLE Masters_Drawings ADD VersionNumber INT NULL DEFAULT 1;
    PRINT 'Added VersionNumber column';
END

-- Add Remarks
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'Remarks')
BEGIN
    ALTER TABLE Masters_Drawings ADD Remarks NVARCHAR(500) NULL;
    -- Copy from Description if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'Description')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET Remarks = Description';
    END
    PRINT 'Added Remarks column';
END

-- Add CreatedAt if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Masters_Drawings ADD CreatedAt DATETIME NOT NULL DEFAULT GETDATE();
    -- Copy from UploadDate if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'UploadDate')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET CreatedAt = UploadDate';
    END
    PRINT 'Added CreatedAt column';
END

-- Add CreatedBy if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Masters_Drawings ADD CreatedBy NVARCHAR(100) NULL;
    -- Copy from UploadedBy if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'UploadedBy')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET CreatedBy = UploadedBy';
    END
    PRINT 'Added CreatedBy column';
END

-- Add UpdatedAt
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Masters_Drawings ADD UpdatedAt DATETIME NULL;
    PRINT 'Added UpdatedAt column';
END

-- Add UpdatedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Masters_Drawings ADD UpdatedBy NVARCHAR(100) NULL;
    PRINT 'Added UpdatedBy column';
END

GO

PRINT '';
PRINT '=== Verification: Current table structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Drawings'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT 'All columns added successfully to Masters_Drawings table!';
GO
