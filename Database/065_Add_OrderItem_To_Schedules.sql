-- Migration 065: Add Order and OrderItem fields to Scheduling_MachineSchedules
-- Purpose: Support multi-product orders - link schedules to specific order items
-- Date: 2026-02-15

USE MultiHitechERP
GO

-- Add OrderId column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Scheduling_MachineSchedules'
               AND COLUMN_NAME = 'OrderId')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    ADD OrderId INT NULL;

    PRINT 'Added OrderId column to Scheduling_MachineSchedules';
END
ELSE
BEGIN
    PRINT 'OrderId column already exists in Scheduling_MachineSchedules';
END
GO

-- Add OrderNo column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Scheduling_MachineSchedules'
               AND COLUMN_NAME = 'OrderNo')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    ADD OrderNo NVARCHAR(50) NULL;

    PRINT 'Added OrderNo column to Scheduling_MachineSchedules';
END
ELSE
BEGIN
    PRINT 'OrderNo column already exists in Scheduling_MachineSchedules';
END
GO

-- Add OrderItemId column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Scheduling_MachineSchedules'
               AND COLUMN_NAME = 'OrderItemId')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    ADD OrderItemId INT NULL;

    PRINT 'Added OrderItemId column to Scheduling_MachineSchedules';
END
ELSE
BEGIN
    PRINT 'OrderItemId column already exists in Scheduling_MachineSchedules';
END
GO

-- Add ItemSequence column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Scheduling_MachineSchedules'
               AND COLUMN_NAME = 'ItemSequence')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    ADD ItemSequence NVARCHAR(10) NULL;

    PRINT 'Added ItemSequence column to Scheduling_MachineSchedules';
END
ELSE
BEGIN
    PRINT 'ItemSequence column already exists in Scheduling_MachineSchedules';
END
GO

-- Backfill OrderId, OrderNo, OrderItemId, ItemSequence from Planning_JobCards
UPDATE s
SET
    s.OrderId = jc.OrderId,
    s.OrderNo = jc.OrderNo,
    s.OrderItemId = jc.OrderItemId,
    s.ItemSequence = jc.ItemSequence
FROM Scheduling_MachineSchedules s
INNER JOIN Planning_JobCards jc ON s.JobCardId = jc.Id
WHERE s.OrderId IS NULL;
GO

PRINT 'Backfilled Order and OrderItem data from job cards';
GO

-- Add foreign key constraint to Orders
IF NOT EXISTS (SELECT * FROM sys.foreign_keys
               WHERE name = 'FK_Schedule_Order')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    ADD CONSTRAINT FK_Schedule_Order
    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
    ON DELETE NO ACTION;

    PRINT 'Added foreign key FK_Schedule_Order';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_Schedule_Order already exists';
END
GO

-- Add foreign key constraint to OrderItems
IF NOT EXISTS (SELECT * FROM sys.foreign_keys
               WHERE name = 'FK_Schedule_OrderItem')
BEGIN
    ALTER TABLE Scheduling_MachineSchedules
    ADD CONSTRAINT FK_Schedule_OrderItem
    FOREIGN KEY (OrderItemId) REFERENCES Orders_OrderItems(Id)
    ON DELETE NO ACTION;

    PRINT 'Added foreign key FK_Schedule_OrderItem';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_Schedule_OrderItem already exists';
END
GO

-- Add indexes for performance
IF NOT EXISTS (SELECT * FROM sys.indexes
               WHERE name = 'IX_Schedule_OrderId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Schedule_OrderId
    ON Scheduling_MachineSchedules(OrderId)
    WHERE OrderId IS NOT NULL;

    PRINT 'Created index IX_Schedule_OrderId';
END
ELSE
BEGIN
    PRINT 'Index IX_Schedule_OrderId already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes
               WHERE name = 'IX_Schedule_OrderItemId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Schedule_OrderItemId
    ON Scheduling_MachineSchedules(OrderItemId)
    WHERE OrderItemId IS NOT NULL;

    PRINT 'Created index IX_Schedule_OrderItemId';
END
ELSE
BEGIN
    PRINT 'Index IX_Schedule_OrderItemId already exists';
END
GO

PRINT 'Migration 065 completed successfully';
GO
