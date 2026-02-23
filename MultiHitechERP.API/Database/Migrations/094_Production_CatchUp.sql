-- ============================================================
-- Migration 094: Production Catch-Up
-- Combines 091 + 092 + 093 with full idempotent guards.
-- Safe to run on any DB — skips steps already applied.
-- ============================================================


-- ── 091: Scheduling_ShiftMaster ─────────────────────────────
IF OBJECT_ID('dbo.Scheduling_ShiftMaster', 'U') IS NULL
BEGIN
    CREATE TABLE Scheduling_ShiftMaster (
        Id               INT IDENTITY(1,1) PRIMARY KEY,
        ShiftName        NVARCHAR(100)   NOT NULL,
        StartTime        TIME            NOT NULL,
        EndTime          TIME            NOT NULL,
        RegularHours     DECIMAL(5,2)    NOT NULL DEFAULT 8.0,
        MaxOvertimeHours DECIMAL(5,2)    NOT NULL DEFAULT 3.0,
        IsActive         BIT             NOT NULL DEFAULT 1,
        CreatedAt        DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy        NVARCHAR(200)   NULL,
        UpdatedAt        DATETIME2       NULL,
        UpdatedBy        NVARCHAR(200)   NULL
    );
    PRINT 'Created Scheduling_ShiftMaster';

    -- Default shifts
    INSERT INTO Scheduling_ShiftMaster (ShiftName, StartTime, EndTime, RegularHours, MaxOvertimeHours, IsActive)
    VALUES
        ('Shift A',     '08:00:00', '16:00:00', 8.0, 3.0, 1),
        ('Shift B',     '16:00:00', '00:00:00', 8.0, 3.0, 1),
        ('Night Shift', '00:00:00', '08:00:00', 8.0, 3.0, 1);
    PRINT 'Inserted default shifts';
END
ELSE
    PRINT 'Scheduling_ShiftMaster already exists — skipped';
GO


-- ── 092: Shift columns on MachineSchedules ──────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Scheduling_MachineSchedules') AND name = 'ShiftId')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules ADD ShiftId INT NULL;
    PRINT 'Added ShiftId to Scheduling_MachineSchedules';
END
ELSE
    PRINT 'ShiftId already exists — skipped';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Scheduling_MachineSchedules') AND name = 'ShiftName')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules ADD ShiftName NVARCHAR(100) NULL;
    PRINT 'Added ShiftName to Scheduling_MachineSchedules';
END
ELSE
    PRINT 'ShiftName already exists — skipped';
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachineSchedules_Shift')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    ADD CONSTRAINT FK_MachineSchedules_Shift
        FOREIGN KEY (ShiftId) REFERENCES Scheduling_ShiftMaster(Id);
    PRINT 'Added FK_MachineSchedules_Shift';
END
ELSE
    PRINT 'FK_MachineSchedules_Shift already exists — skipped';
GO


-- ── 092: Rework columns on JobCards ─────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'IsRework')
BEGIN
    ALTER TABLE Planning_JobCards ADD IsRework BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsRework to Planning_JobCards';
END
ELSE
    PRINT 'IsRework already exists — skipped';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'ParentJobCardId')
BEGIN
    ALTER TABLE Planning_JobCards ADD ParentJobCardId INT NULL;
    PRINT 'Added ParentJobCardId to Planning_JobCards';
END
ELSE
    PRINT 'ParentJobCardId already exists — skipped';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'JobCardType')
BEGIN
    ALTER TABLE Planning_JobCards ADD JobCardType NVARCHAR(50) NOT NULL DEFAULT 'Normal';
    PRINT 'Added JobCardType to Planning_JobCards';
END
ELSE
    PRINT 'JobCardType already exists — skipped';
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_JobCards_ParentJobCard')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD CONSTRAINT FK_JobCards_ParentJobCard
        FOREIGN KEY (ParentJobCardId) REFERENCES Planning_JobCards(Id);
    PRINT 'Added FK_JobCards_ParentJobCard';
END
ELSE
    PRINT 'FK_JobCards_ParentJobCard already exists — skipped';
GO


-- ── 093: Inventory_Transactions ─────────────────────────────
IF OBJECT_ID('dbo.Inventory_Transactions', 'U') IS NULL
BEGIN
    CREATE TABLE Inventory_Transactions (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        MaterialId      INT            NOT NULL,
        TransactionType NVARCHAR(50)   NOT NULL,
        TransactionNo   NVARCHAR(100)  NOT NULL,
        TransactionDate DATETIME       NOT NULL,
        Quantity        DECIMAL(18,4)  NOT NULL,
        UOM             NVARCHAR(20)   NOT NULL,
        ReferenceType   NVARCHAR(50)   NULL,
        ReferenceId     INT            NULL,
        ReferenceNo     NVARCHAR(100)  NULL,
        FromLocation    NVARCHAR(200)  NULL,
        ToLocation      NVARCHAR(200)  NULL,
        UnitCost        DECIMAL(18,4)  NULL,
        TotalCost       DECIMAL(18,4)  NULL,
        BalanceQuantity DECIMAL(18,4)  NOT NULL DEFAULT 0,
        Remarks         NVARCHAR(500)  NULL,
        PerformedBy     NVARCHAR(100)  NULL,
        JobCardId       INT            NULL,
        RequisitionId   INT            NULL,
        SupplierId      INT            NULL,
        GRNNo           NVARCHAR(100)  NULL,
        CreatedAt       DATETIME       NOT NULL DEFAULT GETDATE(),
        CreatedBy       NVARCHAR(100)  NULL
    );

    CREATE INDEX IX_InvTx_MaterialId      ON Inventory_Transactions (MaterialId);
    CREATE INDEX IX_InvTx_TransactionType ON Inventory_Transactions (TransactionType);
    CREATE INDEX IX_InvTx_TransactionDate ON Inventory_Transactions (TransactionDate DESC);
    CREATE INDEX IX_InvTx_JobCardId       ON Inventory_Transactions (JobCardId);

    PRINT 'Created Inventory_Transactions table';
END
ELSE
    PRINT 'Inventory_Transactions already exists — skipped';
GO


PRINT '=== Migration 094 complete ===';
GO
