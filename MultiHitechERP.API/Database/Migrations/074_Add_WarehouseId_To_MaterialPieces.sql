-- Migration 074: Add WarehouseId FK to Stores_MaterialPieces
-- Links each material piece to a specific warehouse rack location
-- WarehouseId is nullable: existing pieces may not have a warehouse assigned yet
-- The legacy StorageLocation text field is kept for backward compatibility

ALTER TABLE Stores_MaterialPieces
    ADD WarehouseId INT NULL
        CONSTRAINT FK_MaterialPieces_Warehouse
            FOREIGN KEY REFERENCES Stores_MasterWarehouses(Id)
                ON DELETE SET NULL;  -- If warehouse deleted, pieces remain but lose FK

CREATE INDEX IX_MaterialPieces_WarehouseId ON Stores_MaterialPieces(WarehouseId);

-- Optionally denormalize WarehouseName for quick display without JOIN
ALTER TABLE Stores_MaterialPieces
    ADD WarehouseName NVARCHAR(300) NULL;  -- "{Name} - {Rack} {RackNo}" e.g. "Main Warehouse - Rack A R-01"
