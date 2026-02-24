-- Migration 103: Add MaxLengthMM to Masters_Machines
-- Some machines can only process material up to a certain length.
-- NULL means no length restriction.

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Masters_Machines' AND COLUMN_NAME = 'MaxLengthMM'
)
BEGIN
    ALTER TABLE Masters_Machines
    ADD MaxLengthMM DECIMAL(10, 2) NULL;

    PRINT 'Added MaxLengthMM column to Masters_Machines';
END
ELSE
BEGIN
    PRINT 'MaxLengthMM already exists in Masters_Machines';
END
GO
