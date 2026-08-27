-- Widens every JobCardNo-shaped column that's still nvarchar(50) up to nvarchar(100),
-- matching the width already used by Stores_IssueWindowDraftCuts, Stores_CuttingListItems,
-- Stores_RequisitionMaterialChangeLog, and Masters_Operators.CurrentJobCardNo.
--
-- Root cause: generate-job-cards builds JobCardNo as
--   JC-{OrderNo}{ItemSeqSuffix}-{ChildPartTemplate.TemplateCode}-{StepNo}
-- and TemplateCode can itself be long (e.g. "CPT-BLOCK CYLINDER GEAR-369875"),
-- pushing the combined string past 50 chars and failing with
-- "String or binary data would be truncated" on insert.

ALTER TABLE Planning_JobCards ALTER COLUMN JobCardNo NVARCHAR(100) NOT NULL;
ALTER TABLE Planning_JobCardDependencies ALTER COLUMN DependentJobCardNo NVARCHAR(100) NULL;
ALTER TABLE Planning_JobCardDependencies ALTER COLUMN PrerequisiteJobCardNo NVARCHAR(100) NULL;
ALTER TABLE Planning_JobCardMaterialRequirements ALTER COLUMN JobCardNo NVARCHAR(100) NULL;
ALTER TABLE Scheduling_MachineSchedules ALTER COLUMN JobCardNo NVARCHAR(100) NULL;
ALTER TABLE Stores_MaterialIssues ALTER COLUMN JobCardNo NVARCHAR(100) NULL;
ALTER TABLE Stores_MaterialRequisitions ALTER COLUMN JobCardNo NVARCHAR(100) NULL;
ALTER TABLE Stores_MaterialRequisitionItems ALTER COLUMN JobCardNo NVARCHAR(100) NULL;
ALTER TABLE Stores_MaterialUsageHistory ALTER COLUMN JobCardNo NVARCHAR(100) NULL;
ALTER TABLE Production_OSPTracking ALTER COLUMN JobCardNo NVARCHAR(100) NOT NULL;
