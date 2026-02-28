-- Migration 107: Add TotalBilledWeight to Stores_GRN header
-- Stores the sum of vendor-billed weights across all lines (for easy reporting)

ALTER TABLE Stores_GRN
ADD TotalBilledWeight DECIMAL(18,4) NULL;

GO

PRINT 'Migration 107: Added TotalBilledWeight to Stores_GRN';
