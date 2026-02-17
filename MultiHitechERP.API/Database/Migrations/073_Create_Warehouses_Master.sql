
-- Migration 073: Create Warehouses Master Table
-- Stores warehouse/rack locations for raw materials and components
-- Used in inventory to track where materials are physically stored
-- Min stock is tracked in TWO ways: minimum pieces count + minimum total length (mm)

CREATE TABLE Stores_MasterWarehouses (
    Id                  INT PRIMARY KEY IDENTITY(1,1),
    Name                NVARCHAR(200) NOT NULL,           -- Warehouse / location name
    Rack                NVARCHAR(100) NOT NULL,            -- Rack label (e.g., "Rack A", "Steel Section")
    RackNo              NVARCHAR(50)  NOT NULL,            -- Rack number (e.g., "R-01", "A-15")
    MaterialType        NVARCHAR(20)  NOT NULL,            -- "RawMaterial" or "Component"
    MinStockPieces      INT           NOT NULL DEFAULT 0, -- Minimum pieces count alert threshold
    MinStockLengthMM    DECIMAL(18,4) NOT NULL DEFAULT 0, -- Minimum total length (mm) alert threshold
    IsActive            BIT           NOT NULL DEFAULT 1,
    CreatedAt           DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           NVARCHAR(100) NULL,
    UpdatedAt           DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy           NVARCHAR(100) NULL,

    CONSTRAINT CK_MasterWarehouses_MaterialType
        CHECK (MaterialType IN ('RawMaterial', 'Component'))
);

CREATE INDEX IX_MasterWarehouses_MaterialType ON Stores_MasterWarehouses(MaterialType);
CREATE INDEX IX_MasterWarehouses_IsActive     ON Stores_MasterWarehouses(IsActive);
