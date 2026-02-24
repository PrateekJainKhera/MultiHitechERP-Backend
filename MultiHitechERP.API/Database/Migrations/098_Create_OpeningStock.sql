-- ============================================================
-- Migration 098: Opening Stock Entry
-- Allows adding existing raw materials + components to inventory
-- without a PO or vendor. Entry number format: OS-YYYYMM-NNN
-- ============================================================

-- ── Stores_OpeningStockEntries (header) ──────────────────────
IF OBJECT_ID('dbo.Stores_OpeningStockEntries', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_OpeningStockEntries (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        EntryNo         NVARCHAR(50)    NOT NULL UNIQUE,   -- OS-202602-001
        EntryDate       DATETIME        NOT NULL DEFAULT GETDATE(),
        Status          NVARCHAR(20)    NOT NULL DEFAULT 'Draft',  -- Draft / Confirmed
        Remarks         NVARCHAR(500)   NULL,
        CreatedAt       DATETIME        NOT NULL DEFAULT GETDATE(),
        CreatedBy       NVARCHAR(100)   NULL,
        ConfirmedAt     DATETIME        NULL,
        ConfirmedBy     NVARCHAR(100)   NULL,
        TotalPieces     INT             NULL,              -- raw material pieces created
        TotalComponents INT             NULL,              -- component lines confirmed
        CONSTRAINT CHK_OpeningStock_Status CHECK (Status IN ('Draft', 'Confirmed'))
    );
    PRINT 'Created Stores_OpeningStockEntries';
END
ELSE
    PRINT 'Stores_OpeningStockEntries already exists — skipped';
GO


-- ── Stores_OpeningStockItems (lines — both raw material and component) ─
IF OBJECT_ID('dbo.Stores_OpeningStockItems', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_OpeningStockItems (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        EntryId         INT             NOT NULL REFERENCES Stores_OpeningStockEntries(Id) ON DELETE CASCADE,
        SequenceNo      INT             NOT NULL DEFAULT 1,
        ItemType        NVARCHAR(20)    NOT NULL DEFAULT 'RawMaterial',  -- RawMaterial / Component

        -- ── Raw Material fields (mirror of Stores_GRNLines) ──
        MaterialId      INT             NULL,
        MaterialName    NVARCHAR(200)   NULL,
        Grade           NVARCHAR(100)   NULL,
        MaterialType    NVARCHAR(20)    NULL,   -- Rod / Pipe / Sheet / Forged
        Diameter        DECIMAL(10,2)   NULL,
        OuterDiameter   DECIMAL(10,2)   NULL,
        InnerDiameter   DECIMAL(10,2)   NULL,
        Width           DECIMAL(10,2)   NULL,
        Thickness       DECIMAL(10,2)   NULL,
        MaterialDensity DECIMAL(10,4)   NULL,   -- g/cm³
        TotalWeightKG   DECIMAL(10,4)   NULL,
        CalculatedLengthMM DECIMAL(10,2) NULL,
        WeightPerMeterKG   DECIMAL(10,4) NULL,
        NumberOfPieces  INT             NULL,
        LengthPerPieceMM DECIMAL(10,2)  NULL,
        WarehouseId     INT             NULL,

        -- ── Component fields ──────────────────────────────────
        ComponentId     INT             NULL,
        ComponentName   NVARCHAR(200)   NULL,
        PartNumber      NVARCHAR(100)   NULL,
        Quantity        DECIMAL(18,3)   NULL,
        UOM             NVARCHAR(20)    NULL,

        -- ── Common ───────────────────────────────────────────
        UnitCost        DECIMAL(18,4)   NULL,
        LineTotal       DECIMAL(18,2)   NULL,
        Remarks         NVARCHAR(200)   NULL,
        SortOrder       INT             NOT NULL DEFAULT 0,

        CONSTRAINT CHK_OpeningStockItem_Type CHECK (ItemType IN ('RawMaterial', 'Component'))
    );

    CREATE INDEX IX_OpeningStockItems_EntryId ON Stores_OpeningStockItems(EntryId);
    PRINT 'Created Stores_OpeningStockItems';
END
ELSE
    PRINT 'Stores_OpeningStockItems already exists — skipped';
GO


PRINT '=== Migration 098 complete ===';
GO
