-- ============================================================
-- Migration 096: Issue Window Drafts Catch-Up (089 + 090)
-- Root cause of cutting-planning Save Draft 500 error:
--   Stores_IssueWindowDrafts and related tables never created
--   on production. Safe to run — fully idempotent.
-- ============================================================


-- ── Stores_IssueWindowDrafts ─────────────────────────────────
IF OBJECT_ID('dbo.Stores_IssueWindowDrafts', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_IssueWindowDrafts (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        DraftNo        NVARCHAR(50)  NOT NULL UNIQUE,
        RequisitionIds NVARCHAR(500) NOT NULL,
        Status         NVARCHAR(20)  NOT NULL DEFAULT 'Draft',
        IssuedBy       NVARCHAR(100) NULL,
        ReceivedBy     NVARCHAR(100) NULL,
        Notes          NVARCHAR(500) NULL,
        CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE(),
        IssuedAt       DATETIME      NULL,
        FinalizedAt    DATETIME      NULL,
        CONSTRAINT CHK_IssueWindowDraft_Status
            CHECK (Status IN ('Draft', 'Finalized', 'Issued'))
    );
    PRINT 'Created Stores_IssueWindowDrafts';
END
ELSE
    PRINT 'Stores_IssueWindowDrafts already exists — skipped';
GO


-- ── FinalizedAt column (090 — in case table existed without it) ─
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Stores_IssueWindowDrafts') AND name = 'FinalizedAt'
)
BEGIN
    ALTER TABLE Stores_IssueWindowDrafts ADD FinalizedAt DATETIME NULL;
    PRINT 'Added FinalizedAt to Stores_IssueWindowDrafts';
END
ELSE
    PRINT 'FinalizedAt already exists — skipped';
GO


-- ── CHECK constraint: drop old (Draft/Issued only) and recreate ─
-- Only needed if table pre-existed without Finalized in constraint
IF EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CHK_IssueWindowDraft_Status'
      AND parent_object_id = OBJECT_ID('Stores_IssueWindowDrafts')
)
BEGIN
    -- Re-create only if Finalized is NOT already allowed
    -- (safe no-op if already correct)
    DECLARE @def NVARCHAR(500);
    SELECT @def = definition
    FROM sys.check_constraints
    WHERE name = 'CHK_IssueWindowDraft_Status';

    IF @def NOT LIKE '%Finalized%'
    BEGIN
        ALTER TABLE Stores_IssueWindowDrafts DROP CONSTRAINT CHK_IssueWindowDraft_Status;
        ALTER TABLE Stores_IssueWindowDrafts
            ADD CONSTRAINT CHK_IssueWindowDraft_Status
            CHECK (Status IN ('Draft', 'Finalized', 'Issued'));
        PRINT 'Updated CHK_IssueWindowDraft_Status to include Finalized';
    END
    ELSE
        PRINT 'CHK_IssueWindowDraft_Status already includes Finalized — skipped';
END
GO


-- ── Stores_IssueWindowDraftBarAssignments ────────────────────
IF OBJECT_ID('dbo.Stores_IssueWindowDraftBarAssignments', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_IssueWindowDraftBarAssignments (
        Id                   INT IDENTITY(1,1) PRIMARY KEY,
        DraftId              INT            NOT NULL REFERENCES Stores_IssueWindowDrafts(Id) ON DELETE CASCADE,
        MaterialId           INT            NULL,
        MaterialName         NVARCHAR(200)  NULL,
        MaterialCode         NVARCHAR(100)  NULL,
        Grade                NVARCHAR(100)  NULL,
        DiameterMM           DECIMAL(10,2)  NULL,
        PieceId              INT            NULL,
        PieceNo              NVARCHAR(100)  NULL,
        PieceCurrentLengthMM DECIMAL(10,2)  NULL,
        TotalCutMM           DECIMAL(10,2)  NULL,
        RemainingMM          DECIMAL(10,2)  NULL,
        WillBeScrap          BIT            NOT NULL DEFAULT 0,
        SortOrder            INT            NOT NULL DEFAULT 0
    );
    CREATE INDEX IX_IssueWindowDraftBarAssignments_DraftId
        ON Stores_IssueWindowDraftBarAssignments(DraftId);
    PRINT 'Created Stores_IssueWindowDraftBarAssignments';
END
ELSE
    PRINT 'Stores_IssueWindowDraftBarAssignments already exists — skipped';
GO


-- ── Stores_IssueWindowDraftCuts ──────────────────────────────
IF OBJECT_ID('dbo.Stores_IssueWindowDraftCuts', 'U') IS NULL
BEGIN
    CREATE TABLE Stores_IssueWindowDraftCuts (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        BarAssignmentId   INT            NOT NULL REFERENCES Stores_IssueWindowDraftBarAssignments(Id) ON DELETE CASCADE,
        DraftId           INT            NOT NULL,
        RequisitionItemId INT            NOT NULL,
        RequisitionId     INT            NULL,
        CutIndex          INT            NOT NULL DEFAULT 0,
        CutLengthMM       DECIMAL(10,2)  NOT NULL,
        PartName          NVARCHAR(200)  NULL,
        JobCardNo         NVARCHAR(100)  NULL,
        RequisitionNo     NVARCHAR(100)  NULL,
        MaterialId        INT            NULL,
        SortOrder         INT            NOT NULL DEFAULT 0
    );
    CREATE INDEX IX_IssueWindowDraftCuts_BarAssignmentId
        ON Stores_IssueWindowDraftCuts(BarAssignmentId);
    CREATE INDEX IX_IssueWindowDraftCuts_DraftId
        ON Stores_IssueWindowDraftCuts(DraftId);
    PRINT 'Created Stores_IssueWindowDraftCuts';
END
ELSE
    PRINT 'Stores_IssueWindowDraftCuts already exists — skipped';
GO


PRINT '=== Migration 096 complete ===';
GO
