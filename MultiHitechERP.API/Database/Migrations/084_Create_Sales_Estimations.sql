-- Migration 084: Create Sales_Estimations table
-- Pre-sales estimation/quotation workflow

CREATE TABLE Sales_Estimations (
    Id                  INT PRIMARY KEY IDENTITY(1,1),
    EstimateNo          NVARCHAR(30)    NOT NULL UNIQUE,     -- EST-202602-001 or EST-202602-001-R2
    BaseEstimateNo      NVARCHAR(20)    NOT NULL,             -- EST-202602-001 (groups all revisions)
    RevisionNumber      INT             NOT NULL DEFAULT 1,
    CustomerId          INT             NOT NULL REFERENCES Masters_Customers(Id),
    CustomerName        NVARCHAR(200)   NULL,
    Status              NVARCHAR(20)    NOT NULL DEFAULT 'Draft',
    -- Financials
    SubTotal            DECIMAL(18,2)   NOT NULL DEFAULT 0,
    DiscountType        NVARCHAR(10)    NULL,                 -- 'Percent' or 'Fixed'
    DiscountValue       DECIMAL(18,2)   NOT NULL DEFAULT 0,
    DiscountAmount      DECIMAL(18,2)   NOT NULL DEFAULT 0,   -- calculated
    TotalAmount         DECIMAL(18,2)   NOT NULL DEFAULT 0,
    -- Validity
    ValidUntil          DATE            NOT NULL,             -- CreatedAt + 21 days
    -- Approval
    ApprovedBy          NVARCHAR(100)   NULL,
    ApprovedAt          DATETIME2       NULL,
    RejectedBy          NVARCHAR(100)   NULL,
    RejectedAt          DATETIME2       NULL,
    RejectionReason     NVARCHAR(500)   NULL,
    -- Conversion
    ConvertedOrderId    INT             NULL REFERENCES Orders(Id),
    ConvertedAt         DATETIME2       NULL,
    -- Content
    Notes               NVARCHAR(1000)  NULL,
    TermsAndConditions  NVARCHAR(2000)  NULL,
    -- Audit
    CreatedAt           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           NVARCHAR(100)   NULL DEFAULT 'Admin',
    CONSTRAINT CHK_Est_Status CHECK (Status IN ('Draft','Submitted','Approved','Rejected','Cancelled','Converted'))
);

CREATE INDEX IX_Sales_Estimations_CustomerId     ON Sales_Estimations(CustomerId);
CREATE INDEX IX_Sales_Estimations_BaseEstimateNo ON Sales_Estimations(BaseEstimateNo);
CREATE INDEX IX_Sales_Estimations_Status         ON Sales_Estimations(Status);

PRINT 'Migration 084: Sales_Estimations table created.';
