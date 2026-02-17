-- Migration 085: Create Sales_EstimationItems table
-- Line items for each estimation

CREATE TABLE Sales_EstimationItems (
    Id              INT PRIMARY KEY IDENTITY(1,1),
    EstimationId    INT             NOT NULL REFERENCES Sales_Estimations(Id) ON DELETE CASCADE,
    ProductId       INT             NOT NULL,
    ProductName     NVARCHAR(200)   NULL,
    PartCode        NVARCHAR(100)   NULL,
    Quantity        INT             NOT NULL,
    UnitPrice       DECIMAL(18,2)   NOT NULL DEFAULT 0,
    TotalPrice      DECIMAL(18,2)   NOT NULL DEFAULT 0,   -- Quantity * UnitPrice
    Notes           NVARCHAR(500)   NULL
);

CREATE INDEX IX_Sales_EstimationItems_EstimationId ON Sales_EstimationItems(EstimationId);

PRINT 'Migration 085: Sales_EstimationItems table created.';
