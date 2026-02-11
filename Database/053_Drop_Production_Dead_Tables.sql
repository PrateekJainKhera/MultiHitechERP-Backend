-- Migration 053: Drop dead Production tables
-- These tables were from an earlier design (separate execution tracking).
-- Production execution is now tracked directly on Planning_JobCards
-- (ProductionStatus, ActualStartTime, ActualEndTime, CompletedQty, RejectedQty, ReadyForAssembly).

-- Drop Production_JobCardExecutions first (it may have FK to Production_JobCards)
IF OBJECT_ID('Production_JobCardExecutions', 'U') IS NOT NULL
BEGIN
    DROP TABLE Production_JobCardExecutions;
    PRINT 'Dropped Production_JobCardExecutions';
END
ELSE
    PRINT 'Production_JobCardExecutions not found — skipping';

-- Drop FKs from other tables that wrongly reference Production_JobCards
-- (these should reference Planning_JobCards instead)

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_QC_JobCard')
BEGIN
    ALTER TABLE Quality_QCResults DROP CONSTRAINT FK_QC_JobCard;
    PRINT 'Dropped FK_QC_JobCard from Quality_QCResults';
END

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MaterialRequisition_JobCard')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions DROP CONSTRAINT FK_MaterialRequisition_JobCard;
    PRINT 'Dropped FK_MaterialRequisition_JobCard from Stores_MaterialRequisitions';
END

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MaterialIssue_JobCard')
BEGIN
    ALTER TABLE Stores_MaterialIssues DROP CONSTRAINT FK_MaterialIssue_JobCard;
    PRINT 'Dropped FK_MaterialIssue_JobCard from Stores_MaterialIssues';
END

-- Drop Production_JobCards
IF OBJECT_ID('Production_JobCards', 'U') IS NOT NULL
BEGIN
    DROP TABLE Production_JobCards;
    PRINT 'Dropped Production_JobCards';
END
ELSE
    PRINT 'Production_JobCards not found — skipping';

-- Re-add FKs pointing to the correct table: Planning_JobCards
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_QC_JobCard')
    AND OBJECT_ID('Quality_QCResults', 'U') IS NOT NULL
BEGIN
    ALTER TABLE Quality_QCResults
    ADD CONSTRAINT FK_QC_JobCard FOREIGN KEY (JobCardId) REFERENCES Planning_JobCards(Id);
    PRINT 'Re-added FK_QC_JobCard → Planning_JobCards';
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MaterialRequisition_JobCard')
    AND OBJECT_ID('Stores_MaterialRequisitions', 'U') IS NOT NULL
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD CONSTRAINT FK_MaterialRequisition_JobCard FOREIGN KEY (JobCardId) REFERENCES Planning_JobCards(Id);
    PRINT 'Re-added FK_MaterialRequisition_JobCard → Planning_JobCards';
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MaterialIssue_JobCard')
    AND OBJECT_ID('Stores_MaterialIssues', 'U') IS NOT NULL
BEGIN
    ALTER TABLE Stores_MaterialIssues
    ADD CONSTRAINT FK_MaterialIssue_JobCard FOREIGN KEY (JobCardId) REFERENCES Planning_JobCards(Id);
    PRINT 'Re-added FK_MaterialIssue_JobCard → Planning_JobCards';
END
