-- Migration 050: Add missing columns to Stores_MaterialIssues
-- The table was originally created with simplified columns; the repository expects richer columns.

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'JobCardNo')
    ALTER TABLE Stores_MaterialIssues ADD JobCardNo NVARCHAR(50) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'OrderNo')
    ALTER TABLE Stores_MaterialIssues ADD OrderNo NVARCHAR(50) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'MaterialName')
    ALTER TABLE Stores_MaterialIssues ADD MaterialName NVARCHAR(200) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'MaterialGrade')
    ALTER TABLE Stores_MaterialIssues ADD MaterialGrade NVARCHAR(100) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'TotalPieces')
    ALTER TABLE Stores_MaterialIssues ADD TotalPieces INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'TotalIssuedLengthMM')
    ALTER TABLE Stores_MaterialIssues ADD TotalIssuedLengthMM DECIMAL(18,3) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'TotalIssuedWeightKG')
    ALTER TABLE Stores_MaterialIssues ADD TotalIssuedWeightKG DECIMAL(18,3) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'IssuedById')
    ALTER TABLE Stores_MaterialIssues ADD IssuedById NVARCHAR(100) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'IssuedByName')
    ALTER TABLE Stores_MaterialIssues ADD IssuedByName NVARCHAR(200) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'ReceivedById')
    ALTER TABLE Stores_MaterialIssues ADD ReceivedById NVARCHAR(100) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Stores_MaterialIssues' AND COLUMN_NAME = 'ReceivedByName')
    ALTER TABLE Stores_MaterialIssues ADD ReceivedByName NVARCHAR(200) NULL;
GO

PRINT 'Migration 050 completed successfully';
