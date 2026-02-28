-- Migration 106: GRN Billed Length + Approval Workflow
-- Stores vendor-billed length/weight separately from actual received values
-- Adds PendingApproval status for >5% variance GRNs

-- 1. Add billed (vendor) values to GRN Lines
ALTER TABLE Stores_GRNLines ADD BilledLengthPerPieceMM DECIMAL(10,2) NULL;
ALTER TABLE Stores_GRNLines ADD BilledWeightKG DECIMAL(10,3) NULL;
ALTER TABLE Stores_GRNLines ADD LengthVariancePct DECIMAL(5,2) NULL;

-- 2. Add approval fields to GRN header
ALTER TABLE Stores_GRN ADD RequiresApproval BIT NOT NULL DEFAULT 0;
ALTER TABLE Stores_GRN ADD ApprovedBy NVARCHAR(100) NULL;
ALTER TABLE Stores_GRN ADD ApprovedAt DATETIME NULL;
ALTER TABLE Stores_GRN ADD ApprovalNotes NVARCHAR(500) NULL;
ALTER TABLE Stores_GRN ADD RejectedBy NVARCHAR(100) NULL;
ALTER TABLE Stores_GRN ADD RejectedAt DATETIME NULL;
ALTER TABLE Stores_GRN ADD RejectionNotes NVARCHAR(500) NULL;

-- 3. Update GRN Status check constraint to include PendingApproval and Rejected
-- Drop existing constraint if any
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'CHK_GRN_Status' AND TABLE_NAME = 'Stores_GRN')
    ALTER TABLE Stores_GRN DROP CONSTRAINT CHK_GRN_Status;

ALTER TABLE Stores_GRN ADD CONSTRAINT CHK_GRN_Status
    CHECK (Status IN ('Draft', 'PendingApproval', 'Received', 'Verified', 'Rejected', 'Cancelled'));
