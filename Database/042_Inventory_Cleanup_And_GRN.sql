-- =====================================================================
-- Migration: 042_Inventory_Cleanup_And_GRN.sql
-- Purpose:
--   1. Drop redundant Inventory_Transactions table
--   2. Update Stores_MaterialPieces with missing columns
--   3. Create GRN tables for material receipt
--   4. Create MaterialUsageHistory for piece cutting tracking
-- =====================================================================

USE MultiHitechERP;
GO

PRINT 'Starting Inventory Cleanup and GRN Migration...';
GO

-- =====================================================================
-- STEP 1: Drop Inventory_Transactions (Redundant)
-- =====================================================================

IF OBJECT_ID('dbo.Inventory_Transactions', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping Inventory_Transactions table...';
    DROP TABLE Inventory_Transactions;
    PRINT 'Inventory_Transactions dropped successfully.';
END
ELSE
BEGIN
    PRINT 'Inventory_Transactions table does not exist. Skipping.';
END
GO

-- =====================================================================
-- STEP 2: Update Stores_MaterialPieces with Missing Columns
-- =====================================================================

PRINT 'Updating Stores_MaterialPieces table...';
GO

-- Drop old table if it exists (from Phase 1 schema)
IF OBJECT_ID('dbo.Inventory_MaterialPieces', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping old Inventory_MaterialPieces table...';
    DROP TABLE Inventory_MaterialPieces;
    PRINT 'Old table dropped.';
END
GO

-- Drop and recreate Stores_MaterialPieces with correct structure
IF OBJECT_ID('dbo.Stores_MaterialPieces', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping existing Stores_MaterialPieces to recreate with correct schema...';
    DROP TABLE Stores_MaterialPieces;
END
GO

CREATE TABLE Stores_MaterialPieces (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PieceNo NVARCHAR(50) NOT NULL UNIQUE,
    MaterialId INT NOT NULL,

    -- Denormalized Material Info (for quick display)
    MaterialCode NVARCHAR(50),
    MaterialName NVARCHAR(200),
    Grade NVARCHAR(100),
    Diameter DECIMAL(10,2), -- in mm

    -- Length & Weight Tracking (CRITICAL - stored in MM and KG)
    OriginalLengthMM DECIMAL(10,2) NOT NULL,
    CurrentLengthMM DECIMAL(10,2) NOT NULL,
    OriginalWeightKG DECIMAL(10,4) NOT NULL,
    CurrentWeightKG DECIMAL(10,4) NOT NULL,

    -- Status
    Status NVARCHAR(20) DEFAULT 'Available',
    -- Status values: Available, Allocated, Issued, InUse, Consumed, Scrap
    AllocatedToRequisitionId INT,
    IssuedToJobCardId INT,

    -- Location
    StorageLocation NVARCHAR(100),
    BinNumber NVARCHAR(50),
    RackNumber NVARCHAR(50),

    -- GRN Reference
    GRNId INT, -- Foreign key to Stores_GRN (will be added after GRN table creation)
    GRNNo NVARCHAR(50),
    ReceivedDate DATETIME DEFAULT GETUTCDATE(),
    SupplierBatchNo NVARCHAR(100),
    SupplierId INT,
    UnitCost DECIMAL(18,2),

    -- Wastage Tracking
    IsWastage BIT DEFAULT 0,
    WastageReason NVARCHAR(500),
    ScrapValue DECIMAL(18,2),

    -- Audit
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    -- Constraints
    CONSTRAINT FK_MaterialPieces_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_Materials(Id),

    CONSTRAINT CHK_MaterialPieces_Length
        CHECK (CurrentLengthMM >= 0 AND CurrentLengthMM <= OriginalLengthMM),

    CONSTRAINT CHK_MaterialPieces_Weight
        CHECK (CurrentWeightKG >= 0 AND CurrentWeightKG <= OriginalWeightKG),

    CONSTRAINT CHK_MaterialPieces_Status
        CHECK (Status IN ('Available', 'Allocated', 'Issued', 'InUse', 'Consumed', 'Scrap'))
);
GO

-- Create Indexes
CREATE INDEX IX_MaterialPieces_Material ON Stores_MaterialPieces(MaterialId);
CREATE INDEX IX_MaterialPieces_Status ON Stores_MaterialPieces(Status);
CREATE INDEX IX_MaterialPieces_Location ON Stores_MaterialPieces(StorageLocation);
CREATE INDEX IX_MaterialPieces_GRN ON Stores_MaterialPieces(GRNNo);
CREATE INDEX IX_MaterialPieces_IsWastage ON Stores_MaterialPieces(IsWastage);
GO

PRINT 'Stores_MaterialPieces table created successfully.';
GO

-- =====================================================================
-- STEP 3: Create GRN (Goods Receipt Note) Tables
-- =====================================================================

PRINT 'Creating GRN tables...';
GO

-- Main GRN Header Table
CREATE TABLE Stores_GRN (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GRNNo NVARCHAR(50) NOT NULL UNIQUE,
    GRNDate DATETIME NOT NULL,

    -- Supplier Info
    SupplierId INT,
    SupplierName NVARCHAR(200),
    SupplierBatchNo NVARCHAR(100),

    -- Purchase Reference
    PONo NVARCHAR(50),
    PODate DATETIME,
    InvoiceNo NVARCHAR(50),
    InvoiceDate DATETIME,

    -- Totals
    TotalPieces INT DEFAULT 0,
    TotalWeight DECIMAL(18,4),
    TotalValue DECIMAL(18,2),

    -- Status
    Status NVARCHAR(20) DEFAULT 'Draft',
    -- Status values: Draft, Received, Verified, Cancelled

    -- Quality Check
    QualityCheckStatus NVARCHAR(20),
    QualityCheckedBy NVARCHAR(100),
    QualityCheckedAt DATETIME,
    QualityRemarks NVARCHAR(500),

    -- General
    Remarks NVARCHAR(500),

    -- Audit
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT CHK_GRN_Status
        CHECK (Status IN ('Draft', 'Received', 'Verified', 'Cancelled'))
);
GO

CREATE INDEX IX_GRN_GRNNo ON Stores_GRN(GRNNo);
CREATE INDEX IX_GRN_Date ON Stores_GRN(GRNDate);
CREATE INDEX IX_GRN_Supplier ON Stores_GRN(SupplierId);
CREATE INDEX IX_GRN_Status ON Stores_GRN(Status);
GO

-- GRN Line Items Table
CREATE TABLE Stores_GRNLines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GRNId INT NOT NULL,
    LineNo INT NOT NULL,

    -- Material Reference
    MaterialId INT NOT NULL,
    MaterialName NVARCHAR(200),
    Grade NVARCHAR(100),

    -- Material Type & Dimensions
    MaterialType NVARCHAR(20) NOT NULL, -- 'Rod', 'Pipe', 'Sheet', 'Forged'
    Diameter DECIMAL(10,2), -- For Rod (solid)
    OuterDiameter DECIMAL(10,2), -- For Pipe
    InnerDiameter DECIMAL(10,2), -- For Pipe
    Width DECIMAL(10,2), -- For Sheet
    Thickness DECIMAL(10,2), -- For Sheet

    -- Material Properties
    MaterialDensity DECIMAL(10,4), -- g/cm³ (7.85 for MS/EN8, 7.9 for SS)

    -- Quantities
    TotalWeightKG DECIMAL(10,4) NOT NULL,
    CalculatedLengthMM DECIMAL(10,2),
    WeightPerMeterKG DECIMAL(10,4),

    -- Piece Breakdown
    NumberOfPieces INT NOT NULL,
    LengthPerPieceMM DECIMAL(10,2),

    -- Pricing
    UnitPrice DECIMAL(18,4), -- Price per KG
    LineTotal DECIMAL(18,2),

    -- Remarks
    Remarks NVARCHAR(500),

    CONSTRAINT FK_GRNLines_GRN FOREIGN KEY (GRNId)
        REFERENCES Stores_GRN(Id) ON DELETE CASCADE,

    CONSTRAINT FK_GRNLines_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_Materials(Id),

    CONSTRAINT CHK_GRNLines_MaterialType
        CHECK (MaterialType IN ('Rod', 'Pipe', 'Sheet', 'Forged')),

    CONSTRAINT UQ_GRNLine UNIQUE (GRNId, LineNo)
);
GO

CREATE INDEX IX_GRNLines_GRN ON Stores_GRNLines(GRNId);
CREATE INDEX IX_GRNLines_Material ON Stores_GRNLines(MaterialId);
GO

PRINT 'GRN tables created successfully.';
GO

-- =====================================================================
-- STEP 4: Add Foreign Key from MaterialPieces to GRN
-- =====================================================================

ALTER TABLE Stores_MaterialPieces
ADD CONSTRAINT FK_MaterialPieces_GRN FOREIGN KEY (GRNId)
    REFERENCES Stores_GRN(Id);
GO

CREATE INDEX IX_MaterialPieces_GRNId ON Stores_MaterialPieces(GRNId);
GO

PRINT 'Foreign key added from MaterialPieces to GRN.';
GO

-- =====================================================================
-- STEP 5: Create Material Usage History Table
-- =====================================================================

PRINT 'Creating Material Usage History table...';
GO

CREATE TABLE Stores_MaterialUsageHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    -- Piece Reference
    MaterialPieceId INT NOT NULL,
    PieceNo NVARCHAR(50),

    -- Order & Part Reference
    OrderId INT,
    OrderNo NVARCHAR(50),
    ChildPartId INT,
    ChildPartName NVARCHAR(200),
    ProductName NVARCHAR(200),

    -- Job Card Reference
    JobCardId INT,
    JobCardNo NVARCHAR(50),

    -- Usage Details
    LengthUsedMM DECIMAL(10,2) NOT NULL,
    LengthRemainingMM DECIMAL(10,2),
    WastageGeneratedMM DECIMAL(10,2) DEFAULT 0,

    -- Cutting Details
    CuttingDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CutByOperator NVARCHAR(100),
    CutByOperatorId INT,
    MachineUsed NVARCHAR(100),
    MachineId INT,

    -- Additional Info
    Notes NVARCHAR(500),

    -- Audit
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_UsageHistory_MaterialPiece FOREIGN KEY (MaterialPieceId)
        REFERENCES Stores_MaterialPieces(Id)
);
GO

CREATE INDEX IX_UsageHistory_Piece ON Stores_MaterialUsageHistory(MaterialPieceId);
CREATE INDEX IX_UsageHistory_Order ON Stores_MaterialUsageHistory(OrderId);
CREATE INDEX IX_UsageHistory_JobCard ON Stores_MaterialUsageHistory(JobCardId);
CREATE INDEX IX_UsageHistory_Date ON Stores_MaterialUsageHistory(CuttingDate);
GO

PRINT 'Material Usage History table created successfully.';
GO

-- =====================================================================
-- STEP 6: Drop Inventory_Stock (If using piece-level only for materials)
-- =====================================================================

-- Keep Inventory_Stock for now - it can be used for finished products
-- We'll only track raw materials in Stores_MaterialPieces
PRINT 'Keeping Inventory_Stock for finished product tracking.';
GO

-- =====================================================================
-- SUMMARY
-- =====================================================================

PRINT '';
PRINT '=============================================================';
PRINT 'Migration 042 Completed Successfully!';
PRINT '=============================================================';
PRINT '';
PRINT 'Changes Applied:';
PRINT '  ✓ Dropped Inventory_Transactions (redundant)';
PRINT '  ✓ Recreated Stores_MaterialPieces with complete schema';
PRINT '  ✓ Created Stores_GRN (Goods Receipt Note header)';
PRINT '  ✓ Created Stores_GRNLines (GRN line items)';
PRINT '  ✓ Created Stores_MaterialUsageHistory (cutting history)';
PRINT '  ✓ Added foreign keys and indexes';
PRINT '';
PRINT 'Storage Format:';
PRINT '  - Lengths stored in MM (millimeters)';
PRINT '  - Weights stored in KG (kilograms)';
PRINT '  - Display in UI as Meters (conversion in frontend)';
PRINT '';
PRINT 'Next Steps:';
PRINT '  1. Update backend C# models';
PRINT '  2. Update repositories and services';
PRINT '  3. Update DTOs and controllers';
PRINT '  4. Update frontend API calls';
PRINT '';
PRINT '=============================================================';
GO
