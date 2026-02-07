-- =====================================================================
-- CLEANUP SCRIPT: Delete All Job Cards and Related Data
-- =====================================================================
-- WARNING: This will delete ALL job cards and their materials!
-- Use only in development/testing environments
-- =====================================================================

BEGIN TRANSACTION;
GO

PRINT 'Starting job card cleanup...'
GO

-- Step 1: Delete Job Card Material Requirements
DECLARE @materialCount INT
SELECT @materialCount = COUNT(*) FROM Planning_JobCardMaterialRequirements
PRINT 'Deleting ' + CAST(@materialCount AS NVARCHAR(10)) + ' material requirements...'

DELETE FROM Planning_JobCardMaterialRequirements
GO

-- Step 2: Delete Job Card Dependencies
DECLARE @depsCount INT
SELECT @depsCount = COUNT(*) FROM Planning_JobCardDependencies
PRINT 'Deleting ' + CAST(@depsCount AS NVARCHAR(10)) + ' job card dependencies...'

DELETE FROM Planning_JobCardDependencies
GO

-- Step 3: Delete Job Cards
DECLARE @jobCardCount INT
SELECT @jobCardCount = COUNT(*) FROM Planning_JobCards
PRINT 'Deleting ' + CAST(@jobCardCount AS NVARCHAR(10)) + ' job cards...'

DELETE FROM Planning_JobCards
GO

-- Step 4: Reset Order Planning Status (Optional - uncomment if needed)
-- UPDATE Orders
-- SET PlanningStatus = 'Not Planned'
-- WHERE PlanningStatus = 'Planned'
-- GO

PRINT 'Cleanup completed successfully!'
PRINT '====================================='
SELECT
    'Job Cards' AS TableName,
    COUNT(*) AS RemainingRecords
FROM Planning_JobCards
UNION ALL
SELECT
    'Material Requirements',
    COUNT(*)
FROM Planning_JobCardMaterialRequirements
UNION ALL
SELECT
    'Dependencies',
    COUNT(*)
FROM Planning_JobCardDependencies
GO

COMMIT TRANSACTION;
GO

PRINT 'All job cards and related data deleted successfully!'
