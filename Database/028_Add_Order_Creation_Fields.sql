-- Migration: Add Order Creation Fields
-- Description: Add fields needed for order creation form (order source, agent, scheduling, drawing linkage, etc.)
-- Note: These are CORE creation fields only, not tracking fields

USE MultiHitechERP;
GO

-- Add Order Source field
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'OrderSource')
BEGIN
    ALTER TABLE Orders ADD OrderSource NVARCHAR(20) NOT NULL DEFAULT 'Direct';
    PRINT 'Column OrderSource added';
END
GO

-- Add Agent Customer ID (for agent orders)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'AgentCustomerId')
BEGIN
    ALTER TABLE Orders ADD AgentCustomerId UNIQUEIDENTIFIER NULL;
    PRINT 'Column AgentCustomerId added';
END
GO

-- Add Agent Commission
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'AgentCommission')
BEGIN
    ALTER TABLE Orders ADD AgentCommission DECIMAL(18,2) NULL;
    PRINT 'Column AgentCommission added';
END
GO

-- Add Scheduling Strategy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'SchedulingStrategy')
BEGIN
    ALTER TABLE Orders ADD SchedulingStrategy NVARCHAR(50) NOT NULL DEFAULT 'Due Date';
    PRINT 'Column SchedulingStrategy added';
END
GO

-- Add Primary Drawing ID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'PrimaryDrawingId')
BEGIN
    ALTER TABLE Orders ADD PrimaryDrawingId UNIQUEIDENTIFIER NULL;
    PRINT 'Column PrimaryDrawingId added';
END
GO

-- Add Drawing Source (customer/company)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'DrawingSource')
BEGIN
    ALTER TABLE Orders ADD DrawingSource NVARCHAR(20) NULL;
    PRINT 'Column DrawingSource added';
END
GO

-- Add Customer Machine
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'CustomerMachine')
BEGIN
    ALTER TABLE Orders ADD CustomerMachine NVARCHAR(200) NULL;
    PRINT 'Column CustomerMachine added';
END
GO

-- Add Material Grade Remark
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'MaterialGradeRemark')
BEGIN
    ALTER TABLE Orders ADD MaterialGradeRemark NVARCHAR(100) NULL;
    PRINT 'Column MaterialGradeRemark added';
END
GO

-- Add Linked Product Template ID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'LinkedProductTemplateId')
BEGIN
    ALTER TABLE Orders ADD LinkedProductTemplateId UNIQUEIDENTIFIER NULL;
    PRINT 'Column LinkedProductTemplateId added';
END
GO

-- Add check constraints
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CHK_Orders_OrderSource')
BEGIN
    ALTER TABLE Orders ADD CONSTRAINT CHK_Orders_OrderSource
        CHECK (OrderSource IN ('Direct', 'Agent', 'Dealer', 'Distributor'));
    PRINT 'Constraint CHK_Orders_OrderSource added';
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CHK_Orders_DrawingSource')
BEGIN
    ALTER TABLE Orders ADD CONSTRAINT CHK_Orders_DrawingSource
        CHECK (DrawingSource IS NULL OR DrawingSource IN ('customer', 'company'));
    PRINT 'Constraint CHK_Orders_DrawingSource added';
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CHK_Orders_SchedulingStrategy')
BEGIN
    ALTER TABLE Orders ADD CONSTRAINT CHK_Orders_SchedulingStrategy
        CHECK (SchedulingStrategy IN ('Due Date', 'Priority Flag', 'Customer Importance', 'Resource Availability'));
    PRINT 'Constraint CHK_Orders_SchedulingStrategy added';
END
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

-- Create index on OrderSource for filtering
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_OrderSource' AND object_id = OBJECT_ID('Orders'))
BEGIN
    CREATE INDEX IX_Orders_OrderSource ON Orders(OrderSource);
    PRINT 'Index IX_Orders_OrderSource created';
END
GO

-- Create index on AgentCustomerId for agent order lookups
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_AgentCustomer' AND object_id = OBJECT_ID('Orders'))
BEGIN
    CREATE INDEX IX_Orders_AgentCustomer ON Orders(AgentCustomerId);
    PRINT 'Index IX_Orders_AgentCustomer created';
END
GO

PRINT 'Migration 028: Order creation fields added successfully';
GO
