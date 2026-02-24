-- Migration 099: Add SourceRef column to Inventory_Stock
-- Tracks the source document (e.g., OS-202602-001 for Opening Stock, GRN-202602-001 for GRN Inward)
-- so we can badge components and raw materials by their origin in the UI.

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Inventory_Stock' AND COLUMN_NAME = 'SourceRef'
)
BEGIN
    ALTER TABLE Inventory_Stock
    ADD SourceRef NVARCHAR(50) NULL;

    PRINT 'Added SourceRef column to Inventory_Stock';
END
ELSE
BEGIN
    PRINT 'SourceRef column already exists in Inventory_Stock';
END
GO
