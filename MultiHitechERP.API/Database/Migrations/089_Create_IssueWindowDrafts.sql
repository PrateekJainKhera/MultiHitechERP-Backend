-- Migration 089: Create Issue Window Drafts tables
-- Supports the Material Issue Window draft/cutting slip workflow

CREATE TABLE Stores_IssueWindowDrafts (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    DraftNo        NVARCHAR(50)  NOT NULL UNIQUE,   -- MIS-202602-001
    RequisitionIds NVARCHAR(500) NOT NULL,            -- comma-separated requisition IDs
    Status         NVARCHAR(20)  NOT NULL DEFAULT 'Draft',  -- Draft / Issued
    IssuedBy       NVARCHAR(100) NULL,
    ReceivedBy     NVARCHAR(100) NULL,
    Notes          NVARCHAR(500) NULL,
    CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE(),
    IssuedAt       DATETIME      NULL,
    CONSTRAINT CHK_IssueWindowDraft_Status CHECK (Status IN ('Draft','Issued'))
);

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

CREATE INDEX IX_IssueWindowDraftBarAssignments_DraftId ON Stores_IssueWindowDraftBarAssignments(DraftId);
CREATE INDEX IX_IssueWindowDraftCuts_BarAssignmentId   ON Stores_IssueWindowDraftCuts(BarAssignmentId);
CREATE INDEX IX_IssueWindowDraftCuts_DraftId           ON Stores_IssueWindowDraftCuts(DraftId);
