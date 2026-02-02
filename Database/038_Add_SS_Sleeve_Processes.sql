-- Migration: Add SS Sleeve Manufacturing Processes
-- Purpose: Add processes required for SS Sleeve manufacturing
-- Date: 2026-02-02

USE MultiHitechERP;
GO

PRINT 'Adding SS Sleeve manufacturing processes...';

-- SS Finish Turning
IF NOT EXISTS (SELECT * FROM Masters_Processes WHERE ProcessName = 'SS Finish Turning')
BEGIN
    INSERT INTO Masters_Processes (ProcessName, ProcessCode, Description, Category, IsActive, CreatedAt, UpdatedAt, CreatedBy)
    VALUES (
        'SS Finish Turning',
        'PROC-SS-FINISH-TURN',
        'Precision finish turning operation for stainless steel sleeves to achieve final dimensions and surface finish',
        'Machining',
        1,
        GETDATE(),
        GETDATE(),
        'Admin'
    );
    PRINT '✓ Added SS Finish Turning';
END
ELSE
BEGIN
    PRINT '✓ SS Finish Turning already exists';
END

-- SS Boring
IF NOT EXISTS (SELECT * FROM Masters_Processes WHERE ProcessName = 'SS Boring')
BEGIN
    INSERT INTO Masters_Processes (ProcessName, ProcessCode, Description, Category, IsActive, CreatedAt, UpdatedAt, CreatedBy)
    VALUES (
        'SS Boring',
        'PROC-SS-BORING',
        'Boring operation to create or finish internal diameter of stainless steel hollow sleeves',
        'Machining',
        1,
        GETDATE(),
        GETDATE(),
        'Admin'
    );
    PRINT '✓ Added SS Boring';
END
ELSE
BEGIN
    PRINT '✓ SS Boring already exists';
END

-- SS Grinding
IF NOT EXISTS (SELECT * FROM Masters_Processes WHERE ProcessName = 'SS Grinding')
BEGIN
    INSERT INTO Masters_Processes (ProcessName, ProcessCode, Description, Category, IsActive, CreatedAt, UpdatedAt, CreatedBy)
    VALUES (
        'SS Grinding',
        'PROC-SS-GRINDING',
        'Precision grinding operation for achieving tight tolerances and superior surface finish on stainless steel parts',
        'Grinding',
        1,
        GETDATE(),
        GETDATE(),
        'Admin'
    );
    PRINT '✓ Added SS Grinding';
END
ELSE
BEGIN
    PRINT '✓ SS Grinding already exists';
END

-- SS Quality Inspection
IF NOT EXISTS (SELECT * FROM Masters_Processes WHERE ProcessName = 'SS Quality Inspection')
BEGIN
    INSERT INTO Masters_Processes (ProcessName, ProcessCode, Description, Category, IsActive, CreatedAt, UpdatedAt, CreatedBy)
    VALUES (
        'SS Quality Inspection',
        'PROC-SS-QC',
        'Final quality inspection including dimensional checks, surface finish verification, and visual inspection for SS parts',
        'Quality Control',
        1,
        GETDATE(),
        GETDATE(),
        'Admin'
    );
    PRINT '✓ Added SS Quality Inspection';
END
ELSE
BEGIN
    PRINT '✓ SS Quality Inspection already exists';
END

GO

PRINT 'Migration completed successfully!';
PRINT 'Added 4 processes for SS Sleeve manufacturing';
GO
