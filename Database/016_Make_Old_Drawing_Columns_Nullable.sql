-- Make old Drawing columns nullable to avoid conflicts
USE MultiHitechERP;
GO

PRINT 'Making old Drawing columns nullable...';
GO

-- Make DrawingNo nullable (we use DrawingNumber now)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'DrawingNo')
BEGIN
    ALTER TABLE Masters_Drawings ALTER COLUMN DrawingNo NVARCHAR(100) NULL;
    PRINT 'Made DrawingNo column nullable';
END

-- Make DrawingName nullable (we use DrawingTitle now)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'DrawingName')
BEGIN
    ALTER TABLE Masters_Drawings ALTER COLUMN DrawingName NVARCHAR(200) NULL;
    PRINT 'Made DrawingName column nullable';
END

-- Make RevisionNo nullable (we use RevisionNumber now)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'RevisionNo')
BEGIN
    ALTER TABLE Masters_Drawings ALTER COLUMN RevisionNo NVARCHAR(20) NULL;
    PRINT 'Made RevisionNo column nullable';
END

GO

PRINT '';
PRINT 'Old Drawing columns are now nullable!';
GO
