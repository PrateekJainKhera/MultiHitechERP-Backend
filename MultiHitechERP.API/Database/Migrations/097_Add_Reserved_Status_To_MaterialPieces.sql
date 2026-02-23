-- ============================================================
-- Migration 097: Add 'Reserved' status to Stores_MaterialPieces
-- Root cause of cutting-planning Save Draft 500 error:
--   UPDATE Stores_MaterialPieces SET Status = 'Reserved'
--   violates CHECK constraint — 'Reserved' was not a valid status.
-- Fix: Drop old constraint, recreate including 'Reserved'.
-- ============================================================

-- Step 1: Drop existing CHECK constraint on Status (any name)
DECLARE @constraintName NVARCHAR(200);

SELECT @constraintName = cc.name
FROM sys.check_constraints cc
JOIN sys.columns c ON cc.parent_object_id = c.object_id
                   AND cc.parent_column_id = c.column_id
WHERE cc.parent_object_id = OBJECT_ID('Stores_MaterialPieces')
  AND c.name = 'Status';

IF @constraintName IS NOT NULL
BEGIN
    -- Only drop if 'Reserved' is NOT already in the definition
    DECLARE @def NVARCHAR(1000);
    SELECT @def = definition
    FROM sys.check_constraints
    WHERE name = @constraintName;

    IF @def NOT LIKE '%Reserved%'
    BEGIN
        EXEC('ALTER TABLE Stores_MaterialPieces DROP CONSTRAINT [' + @constraintName + ']');
        PRINT 'Dropped old Status CHECK constraint: ' + @constraintName;

        -- Step 2: Add new constraint that includes Reserved
        ALTER TABLE Stores_MaterialPieces
        ADD CONSTRAINT CHK_MaterialPiece_Status
        CHECK (Status IN ('Available', 'Reserved', 'Allocated', 'Issued', 'InUse', 'Consumed', 'Scrap'));
        PRINT 'Added new Status CHECK constraint including Reserved';
    END
    ELSE
        PRINT 'Reserved already in constraint — skipped';
END
ELSE
BEGIN
    -- No constraint exists — nothing to update (Reserved is already allowed)
    PRINT 'No Status CHECK constraint on Stores_MaterialPieces — Reserved is already allowed';
END
GO


PRINT '=== Migration 097 complete ===';
GO
