-- Migration 101: Make Diameter and Length nullable in Masters_Products
-- Allows products to be created without specifying dimensions upfront.

IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Masters_Products' AND COLUMN_NAME = 'Diameter'
    AND IS_NULLABLE = 'NO'
)
BEGIN
    ALTER TABLE Masters_Products
    ALTER COLUMN Diameter DECIMAL(18, 4) NULL;

    PRINT 'Made Diameter nullable in Masters_Products';
END
ELSE
BEGIN
    PRINT 'Diameter is already nullable in Masters_Products';
END
GO

IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Masters_Products' AND COLUMN_NAME = 'Length'
    AND IS_NULLABLE = 'NO'
)
BEGIN
    ALTER TABLE Masters_Products
    ALTER COLUMN Length DECIMAL(18, 4) NULL;

    PRINT 'Made Length nullable in Masters_Products';
END
ELSE
BEGIN
    PRINT 'Length is already nullable in Masters_Products';
END
GO
