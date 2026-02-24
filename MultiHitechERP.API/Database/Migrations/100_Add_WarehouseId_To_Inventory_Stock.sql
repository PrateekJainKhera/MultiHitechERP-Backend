-- Migration 100: Add WarehouseId FK to Inventory_Stock
-- Links component and raw material stock rows to a physical warehouse location.
-- Closes the gap where MaterialPieces had WarehouseId but Inventory_Stock did not.

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Inventory_Stock' AND COLUMN_NAME = 'WarehouseId'
)
BEGIN
    ALTER TABLE Inventory_Stock
    ADD WarehouseId INT NULL
        CONSTRAINT FK_InventoryStock_Warehouse
        REFERENCES Stores_MasterWarehouses(Id);

    PRINT 'Added WarehouseId FK to Inventory_Stock';
END
ELSE
BEGIN
    PRINT 'WarehouseId already exists in Inventory_Stock';
END
GO
