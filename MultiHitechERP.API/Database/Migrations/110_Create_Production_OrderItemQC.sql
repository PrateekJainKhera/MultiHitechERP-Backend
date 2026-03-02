-- Migration 110: Create Production_OrderItemQC table
-- Final QC check at order-item level after assembly is complete.
-- A QC record must have Status='Passed' before the item appears in Ready for Dispatch.

CREATE TABLE Production_OrderItemQC (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    OrderItemId   INT NOT NULL,
    OrderId       INT NOT NULL,

    -- Status: Pending | Passed | Failed
    QCStatus      NVARCHAR(50) NOT NULL DEFAULT 'Pending',

    -- Optional PDF certificate (S3 path)
    CertificatePath NVARCHAR(500) NULL,

    -- Who did the QC and when
    QCCompletedAt   DATETIME NULL,
    QCCompletedBy   NVARCHAR(200) NULL,

    Notes         NVARCHAR(1000) NULL,

    CreatedAt     DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt     DATETIME NULL,

    CONSTRAINT FK_OrderItemQC_OrderItems FOREIGN KEY (OrderItemId) REFERENCES Orders_OrderItems(Id),
    CONSTRAINT FK_OrderItemQC_Orders     FOREIGN KEY (OrderId)     REFERENCES Orders(Id),
    CONSTRAINT CHK_OrderItemQC_Status    CHECK (QCStatus IN ('Pending','Passed','Failed'))
);

CREATE INDEX IX_OrderItemQC_OrderItemId ON Production_OrderItemQC (OrderItemId);
CREATE INDEX IX_OrderItemQC_QCStatus    ON Production_OrderItemQC (QCStatus);
