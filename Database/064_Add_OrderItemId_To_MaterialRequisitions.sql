-- Migration 064: Add OrderItemId and ItemSequence to Stores_MaterialRequisitions
-- Purpose: Support multi-product orders - link material requisitions to specific order items
-- Date: 2026-02-15

USE MultiHitechERP
GO

-- Add OrderItemId and ItemSequence columns
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Stores_MaterialRequisitions'
               AND COLUMN_NAME = 'OrderItemId')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD OrderItemId INT NULL;

    PRINT 'Added OrderItemId column to Stores_MaterialRequisitions';
END
ELSE
BEGIN
    PRINT 'OrderItemId column already exists in Stores_MaterialRequisitions';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Stores_MaterialRequisitions'
               AND COLUMN_NAME = 'ItemSequence')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD ItemSequence NVARCHAR(10) NULL;

    PRINT 'Added ItemSequence column to Stores_MaterialRequisitions';
END
ELSE
BEGIN
    PRINT 'ItemSequence column already exists in Stores_MaterialRequisitions';
END
GO

-- Add foreign key constraint to Orders_OrderItems
IF NOT EXISTS (SELECT * FROM sys.foreign_keys
               WHERE name = 'FK_MaterialRequisitions_OrderItems')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD CONSTRAINT FK_MaterialRequisitions_OrderItems
    FOREIGN KEY (OrderItemId) REFERENCES Orders_OrderItems(Id)
    ON DELETE NO ACTION;

    PRINT 'Added foreign key FK_MaterialRequisitions_OrderItems';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_MaterialRequisitions_OrderItems already exists';
END
GO

-- Add index for performance
IF NOT EXISTS (SELECT * FROM sys.indexes
               WHERE name = 'IX_MaterialRequisitions_OrderItemId')
BEGIN
    SET QUOTED_IDENTIFIER ON;
    SET ANSI_NULLS ON;

    CREATE NONCLUSTERED INDEX IX_MaterialRequisitions_OrderItemId
    ON Stores_MaterialRequisitions(OrderItemId)
    WHERE OrderItemId IS NOT NULL;

    PRINT 'Created index IX_MaterialRequisitions_OrderItemId';
END
ELSE
BEGIN
    PRINT 'Index IX_MaterialRequisitions_OrderItemId already exists';
END
GO

PRINT 'Migration 064 completed successfully';
GO
