-- Add DrawingNumber column to Masters_Drawings table
USE MultiHitechERP;
GO

PRINT 'Adding DrawingNumber column to Masters_Drawings table...';
GO

-- Add DrawingNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'DrawingNumber')
BEGIN
    ALTER TABLE Masters_Drawings ADD DrawingNumber NVARCHAR(100) NULL;
    -- Copy from DrawingNo if exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Masters_Drawings') AND name = 'DrawingNo')
    BEGIN
        EXEC sp_executesql N'UPDATE Masters_Drawings SET DrawingNumber = DrawingNo';
    END
    PRINT 'Added DrawingNumber column and copied data from DrawingNo';
END
ELSE
BEGIN
    PRINT 'DrawingNumber column already exists';
END

GO

PRINT '';
PRINT 'DrawingNumber column added successfully!';
GO
