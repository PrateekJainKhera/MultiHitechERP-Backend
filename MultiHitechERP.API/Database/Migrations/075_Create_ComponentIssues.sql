-- Migration 075: Create Stores_ComponentIssues table
-- Manual component issue window — storekeeper issues boxes/units of components.
-- Stock is deducted only when this record is created, NOT on job card creation.

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stores_ComponentIssues')
BEGIN
    CREATE TABLE Stores_ComponentIssues (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        IssueNo         NVARCHAR(50)  NOT NULL,   -- e.g. CI-20260217-001
        IssueDate       DATETIME2     NOT NULL DEFAULT GETUTCDATE(),

        -- Component
        ComponentId     INT           NOT NULL,
        ComponentName   NVARCHAR(200) NOT NULL,
        PartNumber      NVARCHAR(100) NULL,
        Unit            NVARCHAR(20)  NOT NULL DEFAULT 'Pcs',

        -- Quantity
        IssuedQty       DECIMAL(18,3) NOT NULL,

        -- Who
        RequestedBy     NVARCHAR(200) NOT NULL,
        IssuedBy        NVARCHAR(200) NOT NULL,

        -- Notes
        Remarks         NVARCHAR(500) NULL,

        -- Audit
        CreatedAt       DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy       NVARCHAR(200) NULL,

        CONSTRAINT FK_ComponentIssues_Component
            FOREIGN KEY (ComponentId) REFERENCES Masters_Components(Id)
    );

    CREATE INDEX IX_ComponentIssues_ComponentId ON Stores_ComponentIssues(ComponentId);
    CREATE INDEX IX_ComponentIssues_IssueDate    ON Stores_ComponentIssues(IssueDate);
END
GO

PRINT 'Migration 075: Stores_ComponentIssues created successfully';
GO
