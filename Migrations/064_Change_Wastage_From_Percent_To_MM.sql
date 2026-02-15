-- ================================================================
-- Migration 064: Change Wastage from Percent to MM
-- ================================================================
-- Problem: Wastage is currently defined in percentage (%)
-- Solution: Change to millimeters (mm) with default 5mm
-- ================================================================

PRINT 'Starting Migration 064: Change Wastage from Percent to MM';

-- Step 1: Rename column in Masters_ChildPartTemplateMaterialRequirements
EXEC sp_rename 'Masters_ChildPartTemplateMaterialRequirements.WastagePercent', 'WastageMM', 'COLUMN';
PRINT 'Step 1: Renamed WastagePercent to WastageMM in Masters_ChildPartTemplateMaterialRequirements';
GO

-- Step 2: Update default constraint for Masters_ChildPartTemplateMaterialRequirements
DECLARE @sql NVARCHAR(MAX);
DECLARE @constraintName NVARCHAR(256);

SELECT @constraintName = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
WHERE t.name = 'Masters_ChildPartTemplateMaterialRequirements'
  AND c.name = 'WastageMM';

IF @constraintName IS NOT NULL
BEGIN
    SET @sql = 'ALTER TABLE Masters_ChildPartTemplateMaterialRequirements DROP CONSTRAINT ' + @constraintName;
    EXEC sp_executesql @sql;
    PRINT 'Dropped old default constraint for Masters_ChildPartTemplateMaterialRequirements.WastageMM';
END

ALTER TABLE Masters_ChildPartTemplateMaterialRequirements
ADD CONSTRAINT DF_ChildPartTemplate_WastageMM DEFAULT (5) FOR WastageMM;
PRINT 'Step 2: Set default to 5mm for Masters_ChildPartTemplateMaterialRequirements.WastageMM';

-- Step 3: Update existing rows to use 5mm instead of 0% (since % doesn't make sense for mm)
UPDATE Masters_ChildPartTemplateMaterialRequirements
SET WastageMM = 5
WHERE WastageMM = 0;
PRINT 'Step 3: Updated existing rows in Masters_ChildPartTemplateMaterialRequirements to 5mm';
GO

-- Step 4: Rename column in Planning_JobCardMaterialRequirements
EXEC sp_rename 'Planning_JobCardMaterialRequirements.WastagePercent', 'WastageMM', 'COLUMN';
PRINT 'Step 4: Renamed WastagePercent to WastageMM in Planning_JobCardMaterialRequirements';
GO

-- Step 5: Update default constraint for Planning_JobCardMaterialRequirements
DECLARE @sql NVARCHAR(MAX);
DECLARE @constraintName NVARCHAR(256);

SELECT @constraintName = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
WHERE t.name = 'Planning_JobCardMaterialRequirements'
  AND c.name = 'WastageMM';

IF @constraintName IS NOT NULL
BEGIN
    SET @sql = 'ALTER TABLE Planning_JobCardMaterialRequirements DROP CONSTRAINT ' + @constraintName;
    EXEC sp_executesql @sql;
    PRINT 'Dropped old default constraint for Planning_JobCardMaterialRequirements.WastageMM';
END

ALTER TABLE Planning_JobCardMaterialRequirements
ADD CONSTRAINT DF_JobCardMaterialReq_WastageMM DEFAULT (5) FOR WastageMM;
PRINT 'Step 5: Set default to 5mm for Planning_JobCardMaterialRequirements.WastageMM';

-- Step 6: Update existing rows to use 5mm
UPDATE Planning_JobCardMaterialRequirements
SET WastageMM = 5
WHERE WastageMM = 0;
PRINT 'Step 6: Updated existing rows in Planning_JobCardMaterialRequirements to 5mm';
GO

-- Verify the changes
SELECT
    'Masters_ChildPartTemplateMaterialRequirements' AS TableName,
    COUNT(*) AS TotalRows,
    AVG(WastageMM) AS AvgWastageMM,
    MIN(WastageMM) AS MinWastageMM,
    MAX(WastageMM) AS MaxWastageMM
FROM Masters_ChildPartTemplateMaterialRequirements

UNION ALL

SELECT
    'Planning_JobCardMaterialRequirements' AS TableName,
    COUNT(*) AS TotalRows,
    AVG(WastageMM) AS AvgWastageMM,
    MIN(WastageMM) AS MinWastageMM,
    MAX(WastageMM) AS MaxWastageMM
FROM Planning_JobCardMaterialRequirements;
GO

PRINT 'Migration 064 completed successfully';
PRINT 'Wastage is now measured in millimeters (mm) with a default of 5mm';
