-- Migration 112: Add RejectedQty to Planning_JobCards (for rework tracking)

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'RejectedQty')
BEGIN
    ALTER TABLE Planning_JobCards ADD RejectedQty INT NOT NULL DEFAULT 0;
    PRINT 'Added RejectedQty to Planning_JobCards';
END
ELSE
    PRINT 'RejectedQty already exists — skipped';
GO
