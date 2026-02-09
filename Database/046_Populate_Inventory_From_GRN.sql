-- =====================================================================
-- Populate Inventory_Stock from Existing GRN Data
-- =====================================================================
-- This script aggregates all GRN lines and creates inventory records
-- =====================================================================

USE MultiHitechERP;
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Populating Inventory_Stock from GRN data...';
GO

-- Clear existing inventory (if any)
TRUNCATE TABLE Inventory_Stock;
GO

-- Aggregate GRN data by Material and populate Inventory_Stock
-- Convert weight (kg) to length (mm) for storage
INSERT INTO Inventory_Stock (
    ItemType,
    ItemId,
    ItemCode,
    ItemName,
    CurrentStock,
    ReservedStock,
    UOM,
    Location,
    MinStockLevel,
    MaxStockLevel,
    LastUpdated,
    UpdatedBy
)
SELECT
    'RawMaterial' AS ItemType,
    gl.MaterialId AS ItemId,
    m.MaterialCode AS ItemCode,
    gl.MaterialName AS ItemName,
    -- Convert total weight to length based on material specifications
    CASE
        -- For Rod/Forged: Length = Weight × 1,000,000 / (Density × π × (diameter/20)²)
        WHEN m.Shape IN ('Rod', 'Forged') THEN
            SUM(gl.TotalWeightKG) * 1000000 / (m.Density * PI() * POWER(m.Diameter / 20.0, 2))
        -- For Pipe: Length = Weight × 1,000,000 / (Density × π × ((outerD/20)² - (innerD/20)²))
        WHEN m.Shape = 'Pipe' THEN
            SUM(gl.TotalWeightKG) * 1000000 / (m.Density * PI() * (POWER(m.Diameter / 20.0, 2) - POWER(ISNULL(m.InnerDiameter, 0) / 20.0, 2)))
        -- For Sheet: keep in kg (no length conversion without thickness)
        ELSE SUM(gl.TotalWeightKG)
    END AS CurrentStock,
    0 AS ReservedStock,
    -- Set UOM based on shape
    CASE WHEN m.Shape IN ('Rod', 'Forged', 'Pipe') THEN 'mm' ELSE 'kg' END AS UOM,
    'Main Warehouse' AS Location,
    0 AS MinStockLevel,
    NULL AS MaxStockLevel, -- Optional - no max limit by default
    GETDATE() AS LastUpdated,
    'System' AS UpdatedBy
FROM Stores_GRNLines gl
INNER JOIN Stores_GRN g ON gl.GRNId = g.Id
LEFT JOIN Masters_Materials m ON gl.MaterialId = m.Id
GROUP BY
    gl.MaterialId,
    m.MaterialCode,
    gl.MaterialName,
    m.Shape,
    m.Diameter,
    m.InnerDiameter,
    m.Density;

GO

-- Show results
PRINT '';
PRINT '=== Inventory Population Results ===';
PRINT 'Note: Raw materials stored in LENGTH (mm) for direct manufacturing use';
PRINT '';
SELECT
    ItemType,
    ItemId,
    ItemCode,
    ItemName,
    CAST(CurrentStock AS DECIMAL(10,2)) AS 'Stock',
    CAST(AvailableStock AS DECIMAL(10,2)) AS 'Available',
    UOM
FROM Inventory_Stock
ORDER BY ItemType, ItemName;

GO

PRINT '';
PRINT 'Inventory_Stock populated successfully from GRN data!';
PRINT '';
GO
