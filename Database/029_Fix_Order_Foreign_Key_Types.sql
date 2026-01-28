-- Migration: Fix Order Foreign Key Data Types
-- Description: Fix AgentCustomerId, PrimaryDrawingId, LinkedProductTemplateId to use INT instead of UNIQUEIDENTIFIER

USE MultiHitechERP;
GO

-- Drop columns with wrong data type
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'AgentCustomerId')
BEGIN
    ALTER TABLE Orders DROP COLUMN AgentCustomerId;
    PRINT 'Dropped AgentCustomerId column';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'PrimaryDrawingId')
BEGIN
    ALTER TABLE Orders DROP COLUMN PrimaryDrawingId;
    PRINT 'Dropped PrimaryDrawingId column';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'LinkedProductTemplateId')
BEGIN
    ALTER TABLE Orders DROP COLUMN LinkedProductTemplateId;
    PRINT 'Dropped LinkedProductTemplateId column';
END
GO

-- Add columns with correct INT data type
ALTER TABLE Orders ADD AgentCustomerId INT NULL;
PRINT 'Added AgentCustomerId as INT';
GO

ALTER TABLE Orders ADD PrimaryDrawingId INT NULL;
PRINT 'Added PrimaryDrawingId as INT';
GO

ALTER TABLE Orders ADD LinkedProductTemplateId INT NULL;
PRINT 'Added LinkedProductTemplateId as INT';
GO

-- Add foreign key for Agent Customer
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_AgentCustomer')
BEGIN
    ALTER TABLE Orders ADD CONSTRAINT FK_Orders_AgentCustomer
        FOREIGN KEY (AgentCustomerId) REFERENCES Masters_Customers(Id);
    PRINT 'Foreign key FK_Orders_AgentCustomer added';
END
GO

-- Add foreign key for Primary Drawing (if Drawings table exists)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_Drawings')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_PrimaryDrawing')
    BEGIN
        ALTER TABLE Orders ADD CONSTRAINT FK_Orders_PrimaryDrawing
            FOREIGN KEY (PrimaryDrawingId) REFERENCES Masters_Drawings(Id);
        PRINT 'Foreign key FK_Orders_PrimaryDrawing added';
    END
END
GO

-- Add foreign key for Linked Product Template
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_ProductTemplate')
BEGIN
    ALTER TABLE Orders ADD CONSTRAINT FK_Orders_ProductTemplate
        FOREIGN KEY (LinkedProductTemplateId) REFERENCES Masters_ProductTemplates(Id);
    PRINT 'Foreign key FK_Orders_ProductTemplate added';
END
GO

-- Re-create index on AgentCustomerId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_AgentCustomer' AND object_id = OBJECT_ID('Orders'))
BEGIN
    CREATE INDEX IX_Orders_AgentCustomer ON Orders(AgentCustomerId);
    PRINT 'Index IX_Orders_AgentCustomer created';
END
GO

PRINT 'Migration 029: Order foreign key types fixed successfully';
GO
