-- Migration: Remove Stock Fields from Materials Master
-- Materials master should only contain material specifications, not inventory data
-- Stock tracking belongs in Inventory module

USE MultiHitechERP;
GO

-- Drop default constraints first
DECLARE @sql NVARCHAR(MAX) = '';

-- Drop default constraint on StockQty
SELECT @sql = @sql + 'ALTER TABLE Masters_Materials DROP CONSTRAINT ' + dc.name + '; '
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
WHERE c.object_id = OBJECT_ID('Masters_Materials') AND c.name = 'StockQty';

-- Drop default constraint on MinStockLevel
SELECT @sql = @sql + 'ALTER TABLE Masters_Materials DROP CONSTRAINT ' + dc.name + '; '
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
WHERE c.object_id = OBJECT_ID('Masters_Materials') AND c.name = 'MinStockLevel';

IF @sql <> ''
BEGIN
    EXEC sp_executesql @sql;
    PRINT 'Default constraints dropped';
END
GO

-- Now drop the columns
ALTER TABLE Masters_Materials DROP COLUMN StockQty;
ALTER TABLE Masters_Materials DROP COLUMN MinStockLevel;
GO

PRINT 'Migration 025: Stock fields removed from Masters_Materials table successfully';
GO
