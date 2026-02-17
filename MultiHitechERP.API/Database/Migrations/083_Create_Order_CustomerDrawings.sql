-- Migration 083: Create Order_CustomerDrawings table
-- Customer-provided drawing files attached to an order (one or many)

CREATE TABLE Order_CustomerDrawings (
    Id              INT             IDENTITY(1,1) PRIMARY KEY,
    OrderId         INT             NOT NULL REFERENCES Orders(Id) ON DELETE CASCADE,
    OriginalFileName NVARCHAR(255)  NOT NULL,
    StoredFileName  NVARCHAR(255)   NOT NULL,  -- UUID-based stored name
    FilePath        NVARCHAR(500)   NOT NULL,
    FileSize        INT             NULL,
    MimeType        NVARCHAR(100)   NULL,
    DrawingType     NVARCHAR(50)    NOT NULL DEFAULT 'other',  -- shaft, tikki, gear, ends, bearing, patti, assembly, other
    Notes           NVARCHAR(500)   NULL,
    UploadedAt      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UploadedBy      NVARCHAR(100)   NULL
);

CREATE INDEX IX_Order_CustomerDrawings_OrderId ON Order_CustomerDrawings(OrderId);

PRINT 'Migration 083: Order_CustomerDrawings table created.';
