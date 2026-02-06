-- Add missing columns to Stores_MaterialRequisitions table
-- to match the repository and model expectations

-- Check if columns exist before adding them
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Stores_MaterialRequisitions') AND name = 'JobCardNo')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD JobCardNo NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Stores_MaterialRequisitions') AND name = 'OrderNo')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD OrderNo NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Stores_MaterialRequisitions') AND name = 'CustomerName')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD CustomerName NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Stores_MaterialRequisitions') AND name = 'Priority')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD Priority NVARCHAR(20) DEFAULT 'Medium' NOT NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Stores_MaterialRequisitions') AND name = 'DueDate')
BEGIN
    ALTER TABLE Stores_MaterialRequisitions
    ADD DueDate DATETIME NULL;
END
GO

-- Ensure RequiredDate column is renamed or compatible
-- Since the model uses DueDate but the original table had RequiredDate
-- We've added DueDate above. If RequiredDate exists and we want to use it:
-- We can keep both or migrate data from RequiredDate to DueDate

PRINT 'Successfully added missing columns to Stores_MaterialRequisitions table';
GO
