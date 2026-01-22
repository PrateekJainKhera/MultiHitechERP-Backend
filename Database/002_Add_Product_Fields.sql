-- Add additional fields to Masters_Products table for enhanced product management
-- Execute this after 001_Phase1_Schema.sql

USE MultiHitechERP;
GO

-- Add new columns to Masters_Products
ALTER TABLE Masters_Products
ADD
    -- Product Classification
    Category NVARCHAR(100),
    ProductType NVARCHAR(100),

    -- Specifications
    Description NVARCHAR(500),
    HSNCode NVARCHAR(20),

    -- Additional Dimensions
    Weight DECIMAL(10,3),
    UOM NVARCHAR(20) DEFAULT 'PCS',

    -- Drawing Reference (keep existing DrawingNo, add DrawingId for FK)
    DrawingId UNIQUEIDENTIFIER,

    -- Pricing
    SellingPrice DECIMAL(18,2),

    -- Production Planning
    MinOrderQuantity INT,

    -- Additional Info
    Remarks NVARCHAR(500);
GO

-- Add foreign key constraint for DrawingId if Masters_Drawings table exists
IF OBJECT_ID('Masters_Drawings', 'U') IS NOT NULL
BEGIN
    ALTER TABLE Masters_Products
    ADD CONSTRAINT FK_Products_Drawing FOREIGN KEY (DrawingId)
        REFERENCES Masters_Drawings(Id);
END
GO

-- Add check constraints for new fields
ALTER TABLE Masters_Products
ADD CONSTRAINT CHK_Products_Weight CHECK (Weight > 0 OR Weight IS NULL);
GO

ALTER TABLE Masters_Products
ADD CONSTRAINT CHK_Products_MinOrderQuantity CHECK (MinOrderQuantity > 0 OR MinOrderQuantity IS NULL);
GO

ALTER TABLE Masters_Products
ADD CONSTRAINT CHK_Products_SellingPrice CHECK (SellingPrice >= 0 OR SellingPrice IS NULL);
GO

PRINT 'Successfully added additional fields to Masters_Products table';
GO
