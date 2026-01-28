-- Migration 031: Create Planning_JobCards and Planning_JobCardDependencies tables
-- Purpose: Core planning tables for job card management
-- Note: Scheduling and Production columns live in their own module tables

BEGIN TRANSACTION;
GO

-- Planning_JobCards: One row per manufacturing operation per order (planning phase only)
CREATE TABLE Planning_JobCards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JobCardNo NVARCHAR(50) NOT NULL UNIQUE,
    CreationType NVARCHAR(50) NOT NULL DEFAULT 'Auto-Generated',  -- Auto-Generated | Manual

    -- Order linkage
    OrderId INT NOT NULL,
    OrderNo NVARCHAR(50) NULL,

    -- Drawing linkage
    DrawingId INT NULL,
    DrawingNumber NVARCHAR(100) NULL,
    DrawingRevision NVARCHAR(10) NULL,
    DrawingName NVARCHAR(200) NULL,
    DrawingSelectionType NVARCHAR(20) NOT NULL DEFAULT 'auto',  -- auto | manual

    -- Child Part linkage
    ChildPartId INT NULL,
    ChildPartName NVARCHAR(200) NULL,
    ChildPartTemplateId INT NULL,

    -- Process linkage
    ProcessId INT NOT NULL,
    ProcessName NVARCHAR(200) NULL,
    ProcessCode NVARCHAR(50) NULL,
    StepNo INT NULL,                          -- Sequence within process template (1, 2, 3...)
    ProcessTemplateId INT NULL,

    -- Instructions
    WorkInstructions NVARCHAR(MAX) NULL,
    QualityCheckpoints NVARCHAR(MAX) NULL,
    SpecialNotes NVARCHAR(MAX) NULL,

    -- Quantity (target for this job card)
    Quantity INT NOT NULL DEFAULT 0,

    -- Planning Status
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',  -- Pending | Planned | Released
    Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium', -- Low | Medium | High | Critical

    -- Manufacturing dimensions (stored as JSON)
    ManufacturingDimensions NVARCHAR(MAX) NULL,

    -- Audit
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(200) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(200) NULL,
    Version INT NOT NULL DEFAULT 1,

    -- Foreign Keys
    CONSTRAINT FK_JobCards_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);

-- Indexes for common queries
CREATE INDEX IX_JobCards_OrderId ON Planning_JobCards (OrderId);
CREATE INDEX IX_JobCards_Status ON Planning_JobCards (Status);
CREATE INDEX IX_JobCards_ProcessId ON Planning_JobCards (ProcessId);

GO

-- Planning_JobCardDependencies: Tracks which job cards must complete before others can start
CREATE TABLE Planning_JobCardDependencies (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    -- The job card that is WAITING (blocked until prerequisite completes)
    DependentJobCardId INT NOT NULL,
    DependentJobCardNo NVARCHAR(50) NULL,

    -- The job card that must COMPLETE first
    PrerequisiteJobCardId INT NOT NULL,
    PrerequisiteJobCardNo NVARCHAR(50) NULL,

    -- Dependency details
    DependencyType NVARCHAR(50) NOT NULL DEFAULT 'Sequential',  -- Sequential | Parallel
    IsResolved BIT NOT NULL DEFAULT 0,
    ResolvedAt DATETIME2 NULL,

    -- Optional lag time after prerequisite completes (e.g., wait 30 min for cooling)
    LagTimeMinutes INT NULL,

    -- Audit
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Foreign Keys
    CONSTRAINT FK_Deps_DependentJobCard FOREIGN KEY (DependentJobCardId) REFERENCES Planning_JobCards(Id),
    CONSTRAINT FK_Deps_PrerequisiteJobCard FOREIGN KEY (PrerequisiteJobCardId) REFERENCES Planning_JobCards(Id),

    -- Unique: each dependency pair can only exist once
    CONSTRAINT UQ_Deps_Pair UNIQUE (DependentJobCardId, PrerequisiteJobCardId)
);

CREATE INDEX IX_Deps_DependentJobCardId ON Planning_JobCardDependencies (DependentJobCardId);
CREATE INDEX IX_Deps_PrerequisiteJobCardId ON Planning_JobCardDependencies (PrerequisiteJobCardId);
CREATE INDEX IX_Deps_IsResolved ON Planning_JobCardDependencies (IsResolved);

GO

COMMIT TRANSACTION;
