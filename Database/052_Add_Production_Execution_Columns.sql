-- Migration 052: Add Production Execution columns to Planning_JobCards
-- These columns track WHAT IS ACTUALLY HAPPENING on the shop floor
-- (as opposed to Planning fields like Status and Priority which are pre-production)

-- ProductionStatus tracks the shop-floor lifecycle:
--   Pending   - Not yet ready to start (previous step not done / not scheduled)
--   Ready     - Can be started by operator (previous step complete, machine scheduled)
--   InProgress - Operator has started work
--   Paused    - Work temporarily stopped
--   Completed - All pieces done, operator confirmed
--
-- ReadyForAssembly: set TRUE when this child part's LAST step is Completed
-- Once all child parts have ReadyForAssembly = 1, the Assembly job card becomes Ready

-- Add columns one at a time so partial failures are easy to diagnose

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'ProductionStatus')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD ProductionStatus NVARCHAR(50) NOT NULL DEFAULT 'Pending';
    PRINT 'Added ProductionStatus column';
END
ELSE
    PRINT 'ProductionStatus already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'ActualStartTime')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD ActualStartTime DATETIME2 NULL;
    PRINT 'Added ActualStartTime column';
END
ELSE
    PRINT 'ActualStartTime already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'ActualEndTime')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD ActualEndTime DATETIME2 NULL;
    PRINT 'Added ActualEndTime column';
END
ELSE
    PRINT 'ActualEndTime already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'CompletedQty')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD CompletedQty INT NOT NULL DEFAULT 0;
    PRINT 'Added CompletedQty column';
END
ELSE
    PRINT 'CompletedQty already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'RejectedQty')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD RejectedQty INT NOT NULL DEFAULT 0;
    PRINT 'Added RejectedQty column';
END
ELSE
    PRINT 'RejectedQty already exists';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'ReadyForAssembly')
BEGIN
    ALTER TABLE Planning_JobCards
    ADD ReadyForAssembly BIT NOT NULL DEFAULT 0;
    PRINT 'Added ReadyForAssembly column';
END
ELSE
    PRINT 'ReadyForAssembly already exists';

GO

-- Seed: First steps of each child part group are Ready (can be started immediately)
-- A first step is: StepNo = 1 for non-assembly job cards that have been scheduled
-- Assembly job cards stay Pending until all child parts are done

UPDATE Planning_JobCards
SET ProductionStatus = 'Ready'
WHERE Status = 'Scheduled'
  AND CreationType != 'Assembly'
  AND StepNo = 1;

PRINT 'Seeded first-step job cards as Ready';
