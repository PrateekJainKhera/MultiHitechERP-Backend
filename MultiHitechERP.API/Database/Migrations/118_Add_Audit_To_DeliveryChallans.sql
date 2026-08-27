-- Audit trail for post-dispatch edits (admin can correct invoice/vehicle/date after dispatch).
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Dispatch_DeliveryChallans') AND name = 'UpdatedAt')
    ALTER TABLE Dispatch_DeliveryChallans ADD UpdatedAt DATETIME2 NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Dispatch_DeliveryChallans') AND name = 'UpdatedBy')
    ALTER TABLE Dispatch_DeliveryChallans ADD UpdatedBy NVARCHAR(255) NULL;
