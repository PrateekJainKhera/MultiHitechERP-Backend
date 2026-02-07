-- Migration 044: Create Planning_JobCardMaterialRequirements table
-- Purpose: Store material requirements for each job card (confirmed during planning)
-- Note: Materials are auto-populated from ChildPartTemplate and confirmed by planner

BEGIN TRANSACTION;
GO

-- Create table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Planning_JobCardMaterialRequirements') AND type in (N'U'))
BEGIN
    CREATE TABLE Planning_JobCardMaterialRequirements (
        Id INT IDENTITY(1,1) PRIMARY KEY,

        -- Job Card linkage
        JobCardId INT NOT NULL,
        JobCardNo NVARCHAR(50) NULL,

        -- Material linkage
        RawMaterialId INT NULL,                      -- FK to Masters_Materials (nullable for manual entry)
        RawMaterialName NVARCHAR(200) NOT NULL,      -- Material name
        MaterialGrade NVARCHAR(100) NOT NULL DEFAULT '',  -- e.g., "SS304", "EN8", "Mild Steel"

        -- Quantity requirements
        RequiredQuantity DECIMAL(18, 4) NOT NULL DEFAULT 0,  -- Base quantity needed
        Unit NVARCHAR(20) NOT NULL DEFAULT 'pcs',             -- pcs, kg, m, sheets, etc.
        WastagePercent DECIMAL(5, 2) NOT NULL DEFAULT 0,      -- Percentage wastage (e.g., 10.00 for 10%)
        TotalQuantityWithWastage DECIMAL(18, 4) NOT NULL DEFAULT 0,  -- RequiredQty * (1 + Wastage/100)

        -- Source tracking
        Source NVARCHAR(50) NOT NULL DEFAULT 'Template',  -- Template | Manual

        -- Confirmation (planner confirms during job card creation)
        ConfirmedBy NVARCHAR(200) NOT NULL DEFAULT '',
        ConfirmedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        -- Audit
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NULL,

        -- Foreign Keys
        CONSTRAINT FK_JobCardMaterialReqs_JobCard FOREIGN KEY (JobCardId)
            REFERENCES Planning_JobCards(Id) ON DELETE CASCADE,
        CONSTRAINT FK_JobCardMaterialReqs_Material FOREIGN KEY (RawMaterialId)
            REFERENCES Masters_Materials(Id) ON DELETE SET NULL
    );

    -- Indexes for common queries
    CREATE INDEX IX_JobCardMaterialReqs_JobCardId ON Planning_JobCardMaterialRequirements (JobCardId);
    CREATE INDEX IX_JobCardMaterialReqs_RawMaterialId ON Planning_JobCardMaterialRequirements (RawMaterialId);
    CREATE INDEX IX_JobCardMaterialReqs_Source ON Planning_JobCardMaterialRequirements (Source);

    PRINT 'Successfully created Planning_JobCardMaterialRequirements table';
END
ELSE
BEGIN
    PRINT 'Table Planning_JobCardMaterialRequirements already exists';
END

GO

COMMIT TRANSACTION;
