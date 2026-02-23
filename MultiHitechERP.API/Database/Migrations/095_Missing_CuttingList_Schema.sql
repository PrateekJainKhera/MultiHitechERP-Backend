-- ============================================================
-- Migration 095: Missing CuttingList Schema (was 088)
-- The original 088 file is missing from the codebase.
-- This recreates it with full idempotent guards.
-- ROOT CAUSE of scheduling 400: IssuedViaCuttingList column
-- missing on Planning_JobCardMaterialRequirements.
-- ============================================================


-- ── Stores_CuttingLists (header table) ──────────────────────
IF OBJECT_ID('dbo.Stores_CuttingLists', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_CuttingLists (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        CuttingListNo NVARCHAR(50)  NOT NULL UNIQUE,
        Status      NVARCHAR(30)   NOT NULL DEFAULT 'Draft',  -- Draft / Issued / Reconciled
        Notes       NVARCHAR(500)  NULL,
        CreatedAt   DATETIME       NOT NULL DEFAULT GETDATE(),
        CreatedBy   NVARCHAR(100)  NULL,
        IssuedAt    DATETIME       NULL,
        IssuedBy    NVARCHAR(100)  NULL,
        ReconciledAt DATETIME      NULL,
        ReconciledBy NVARCHAR(100) NULL
    );
    PRINT 'Created Stores_CuttingLists';
END
ELSE
    PRINT 'Stores_CuttingLists already exists — skipped';
GO


-- ── Stores_CuttingListItems ──────────────────────────────────
IF OBJECT_ID('dbo.Stores_CuttingListItems', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_CuttingListItems (
        Id                  INT IDENTITY(1,1) PRIMARY KEY,
        CuttingListId       INT            NOT NULL REFERENCES Stores_CuttingLists(Id) ON DELETE CASCADE,
        RequirementId       INT            NOT NULL,   -- FK to Planning_JobCardMaterialRequirements
        MaterialId          INT            NULL,
        MaterialName        NVARCHAR(200)  NULL,
        Grade               NVARCHAR(100)  NULL,
        DiameterMM          DECIMAL(10,2)  NULL,
        RequiredLengthMM    DECIMAL(10,2)  NOT NULL,
        WastageMM           DECIMAL(10,2)  NOT NULL DEFAULT 0,
        TotalLengthMM       DECIMAL(10,2)  NOT NULL,
        NumberOfPieces      INT            NOT NULL DEFAULT 1,
        JobCardNo           NVARCHAR(100)  NULL,
        PartName            NVARCHAR(200)  NULL,
        -- Reconciliation
        ActualCutLengthMM   DECIMAL(10,2)  NULL,
        ReconciledAt        DATETIME       NULL,
        SortOrder           INT            NOT NULL DEFAULT 0
    );
    PRINT 'Created Stores_CuttingListItems';
END
ELSE
    PRINT 'Stores_CuttingListItems already exists — skipped';
GO


-- ── Stores_CuttingListReturnPieces ───────────────────────────
IF OBJECT_ID('dbo.Stores_CuttingListReturnPieces', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_CuttingListReturnPieces (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        CuttingListId   INT            NOT NULL REFERENCES Stores_CuttingLists(Id) ON DELETE CASCADE,
        MaterialId      INT            NULL,
        MaterialName    NVARCHAR(200)  NULL,
        LengthMM        DECIMAL(10,2)  NOT NULL,
        IsScrap         BIT            NOT NULL DEFAULT 0,  -- < 300mm = scrap
        PieceId         INT            NULL,                -- created MaterialPiece if returned to stock
        Notes           NVARCHAR(200)  NULL,
        CreatedAt       DATETIME       NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Created Stores_CuttingListReturnPieces';
END
ELSE
    PRINT 'Stores_CuttingListReturnPieces already exists — skipped';
GO


-- ── Planning_JobCardMaterialRequirements: CuttingListId ──────
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Planning_JobCardMaterialRequirements') AND name = 'CuttingListId'
)
BEGIN
    ALTER TABLE Planning_JobCardMaterialRequirements ADD CuttingListId INT NULL;
    PRINT 'Added CuttingListId to Planning_JobCardMaterialRequirements';
END
ELSE
    PRINT 'CuttingListId already exists — skipped';
GO


-- ── Planning_JobCardMaterialRequirements: IssuedViaCuttingList
-- THIS IS THE COLUMN CAUSING THE SCHEDULING 400 ERROR
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Planning_JobCardMaterialRequirements') AND name = 'IssuedViaCuttingList'
)
BEGIN
    ALTER TABLE Planning_JobCardMaterialRequirements ADD IssuedViaCuttingList BIT NOT NULL DEFAULT 0;
    PRINT 'Added IssuedViaCuttingList to Planning_JobCardMaterialRequirements — scheduling 400 should now be fixed';
END
ELSE
    PRINT 'IssuedViaCuttingList already exists — skipped';
GO


PRINT '=== Migration 095 complete ===';
GO
