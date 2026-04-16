-- Migration 113: App_Settings table + GST columns on Sales_Estimations

-- 1. Create App_Settings table
CREATE TABLE App_Settings (
    Id          INT PRIMARY KEY IDENTITY(1,1),
    [Key]       NVARCHAR(100) NOT NULL,
    [Value]     NVARCHAR(500) NOT NULL,
    Description NVARCHAR(500) NULL,
    UpdatedAt   DATETIME2    NULL,
    UpdatedBy   NVARCHAR(100) NULL,
    CONSTRAINT UQ_AppSettings_Key UNIQUE ([Key])
);

-- 2. Seed default GST rate
INSERT INTO App_Settings ([Key], [Value], Description)
VALUES ('GSTRate', '18', 'GST percentage applied to estimations (e.g. 18 = 18%)');

-- 3. Add GST columns to Sales_Estimations
ALTER TABLE Sales_Estimations
ADD GSTRate    DECIMAL(5,2)  NOT NULL DEFAULT 0,
    GSTAmount  DECIMAL(18,2) NOT NULL DEFAULT 0,
    GrandTotal DECIMAL(18,2) NOT NULL DEFAULT 0;

GO

-- 4. Backfill existing records with 18% GST
UPDATE Sales_Estimations
SET GSTRate    = 18,
    GSTAmount  = ROUND(TotalAmount * 18.0 / 100, 2),
    GrandTotal = TotalAmount + ROUND(TotalAmount * 18.0 / 100, 2);

PRINT 'Migration 113: App_Settings created, GST columns added to Sales_Estimations.';
