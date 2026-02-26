-- Migration 104: Create Production_OSPTracking
-- Tracks materials sent to external vendors for OSP (Outside Service Process)
-- e.g., Blackening, Heat Treatment, Anodising, SS Rough Turning

CREATE TABLE Production_OSPTracking (
    Id                  INT             PRIMARY KEY IDENTITY(1,1),

    -- Job Card Reference (denormalized for quick display)
    JobCardId           INT             NOT NULL,
    JobCardNo           NVARCHAR(50)    NOT NULL,

    -- Order Reference
    OrderId             INT             NOT NULL,
    OrderNo             NVARCHAR(50)    NULL,
    OrderItemId         INT             NULL,
    ItemSequence        NVARCHAR(10)    NULL,

    -- Part / Process Info
    ChildPartName       NVARCHAR(200)   NULL,
    ProcessName         NVARCHAR(200)   NULL,

    -- Vendor
    VendorId            INT             NOT NULL,

    -- Quantities
    Quantity            INT             NOT NULL DEFAULT 0,

    -- Dates
    SentDate            DATE            NOT NULL,
    ExpectedReturnDate  DATE            NOT NULL,
    ActualReturnDate    DATE            NULL,

    -- Status
    Status              NVARCHAR(50)    NOT NULL DEFAULT 'Sent',

    -- Notes
    Notes               NVARCHAR(MAX)   NULL,

    -- Audit
    CreatedAt           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           NVARCHAR(100)   NULL,
    UpdatedAt           DATETIME2       NULL,
    UpdatedBy           NVARCHAR(100)   NULL,

    CONSTRAINT FK_OSPTracking_JobCard   FOREIGN KEY (JobCardId) REFERENCES Planning_JobCards(Id),
    CONSTRAINT FK_OSPTracking_Vendor    FOREIGN KEY (VendorId)  REFERENCES Masters_Vendors(Id),
    CONSTRAINT CHK_OSPTracking_Status   CHECK (Status IN ('Sent', 'Received'))
);

-- Index for quick lookup by job card
CREATE INDEX IX_OSPTracking_JobCardId ON Production_OSPTracking(JobCardId);
-- Index for status filtering
CREATE INDEX IX_OSPTracking_Status ON Production_OSPTracking(Status);
