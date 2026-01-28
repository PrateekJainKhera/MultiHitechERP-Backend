-- Migration: Add All Missing Order Columns
-- Description: Add all columns needed to match Order.cs model

USE MultiHitechERP;
GO

-- Add DueDate (rename DeliveryDate)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DeliveryDate')
BEGIN
    EXEC sp_rename 'Orders.DeliveryDate', 'DueDate', 'COLUMN';
    PRINT 'Renamed DeliveryDate to DueDate';
END
GO

-- Add AdjustedDueDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'AdjustedDueDate')
BEGIN
    ALTER TABLE Orders ADD AdjustedDueDate DATETIME NULL;
    PRINT 'Added AdjustedDueDate';
END
GO

-- Add Quantity tracking columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'OriginalQuantity')
BEGIN
    ALTER TABLE Orders ADD OriginalQuantity INT NOT NULL DEFAULT 0;
    PRINT 'Added OriginalQuantity';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'QtyCompleted')
BEGIN
    ALTER TABLE Orders ADD QtyCompleted INT NOT NULL DEFAULT 0;
    PRINT 'Added QtyCompleted';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'QtyRejected')
BEGIN
    ALTER TABLE Orders ADD QtyRejected INT NOT NULL DEFAULT 0;
    PRINT 'Added QtyRejected';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'QtyInProgress')
BEGIN
    ALTER TABLE Orders ADD QtyInProgress INT NOT NULL DEFAULT 0;
    PRINT 'Added QtyInProgress';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'QtyScrap')
BEGIN
    ALTER TABLE Orders ADD QtyScrap INT NOT NULL DEFAULT 0;
    PRINT 'Added QtyScrap';
END
GO

-- Add Planning Status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'PlanningStatus')
BEGIN
    ALTER TABLE Orders ADD PlanningStatus NVARCHAR(20) NOT NULL DEFAULT 'Not Planned';
    PRINT 'Added PlanningStatus';
END
GO

-- Add Drawing Review columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DrawingReviewStatus')
BEGIN
    ALTER TABLE Orders ADD DrawingReviewStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending';
    PRINT 'Added DrawingReviewStatus';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DrawingReviewedBy')
BEGIN
    ALTER TABLE Orders ADD DrawingReviewedBy NVARCHAR(100) NULL;
    PRINT 'Added DrawingReviewedBy';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DrawingReviewedAt')
BEGIN
    ALTER TABLE Orders ADD DrawingReviewedAt DATETIME NULL;
    PRINT 'Added DrawingReviewedAt';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DrawingReviewNotes')
BEGIN
    ALTER TABLE Orders ADD DrawingReviewNotes NVARCHAR(1000) NULL;
    PRINT 'Added DrawingReviewNotes';
END
GO

-- Add Production Tracking columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'CurrentProcess')
BEGIN
    ALTER TABLE Orders ADD CurrentProcess NVARCHAR(200) NULL;
    PRINT 'Added CurrentProcess';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'CurrentMachine')
BEGIN
    ALTER TABLE Orders ADD CurrentMachine NVARCHAR(100) NULL;
    PRINT 'Added CurrentMachine';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'CurrentOperator')
BEGIN
    ALTER TABLE Orders ADD CurrentOperator NVARCHAR(100) NULL;
    PRINT 'Added CurrentOperator';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'ProductionStartDate')
BEGIN
    ALTER TABLE Orders ADD ProductionStartDate DATETIME NULL;
    PRINT 'Added ProductionStartDate';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'ProductionEndDate')
BEGIN
    ALTER TABLE Orders ADD ProductionEndDate DATETIME NULL;
    PRINT 'Added ProductionEndDate';
END
GO

-- Add Rescheduling columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DelayReason')
BEGIN
    ALTER TABLE Orders ADD DelayReason NVARCHAR(100) NULL;
    PRINT 'Added DelayReason';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'RescheduleCount')
BEGIN
    ALTER TABLE Orders ADD RescheduleCount INT NOT NULL DEFAULT 0;
    PRINT 'Added RescheduleCount';
END
GO

-- Add Material Approval columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'MaterialGradeApproved')
BEGIN
    ALTER TABLE Orders ADD MaterialGradeApproved BIT NOT NULL DEFAULT 0;
    PRINT 'Added MaterialGradeApproved';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'MaterialGradeApprovalDate')
BEGIN
    ALTER TABLE Orders ADD MaterialGradeApprovalDate DATETIME NULL;
    PRINT 'Added MaterialGradeApprovalDate';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'MaterialGradeApprovedBy')
BEGIN
    ALTER TABLE Orders ADD MaterialGradeApprovedBy NVARCHAR(100) NULL;
    PRINT 'Added MaterialGradeApprovedBy';
END
GO

-- Add Financial columns (rename existing columns if needed)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'OrderValue')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'TotalAmount')
    BEGIN
        EXEC sp_rename 'Orders.TotalAmount', 'OrderValue', 'COLUMN';
        PRINT 'Renamed TotalAmount to OrderValue';
    END
    ELSE
    BEGIN
        ALTER TABLE Orders ADD OrderValue DECIMAL(18,2) NULL;
        PRINT 'Added OrderValue';
    END
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'AdvancePayment')
BEGIN
    ALTER TABLE Orders ADD AdvancePayment DECIMAL(18,2) NULL;
    PRINT 'Added AdvancePayment';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'BalancePayment')
BEGIN
    ALTER TABLE Orders ADD BalancePayment DECIMAL(18,2) NULL;
    PRINT 'Added BalancePayment';
END
GO

-- Add Version column for optimistic locking
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'Version')
BEGIN
    ALTER TABLE Orders ADD Version INT NOT NULL DEFAULT 1;
    PRINT 'Added Version';
END
GO

-- Update OriginalQuantity with current Quantity values
UPDATE Orders SET OriginalQuantity = Quantity WHERE OriginalQuantity = 0;
PRINT 'Updated OriginalQuantity with Quantity values';
GO

PRINT 'Migration 030: All missing order columns added successfully';
GO
