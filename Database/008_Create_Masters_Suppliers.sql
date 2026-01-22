-- Create Masters_Suppliers table
USE MultiHitechERP;
GO

-- Check if table already exists
IF OBJECT_ID('Masters_Suppliers', 'U') IS NOT NULL
BEGIN
    PRINT 'Masters_Suppliers table already exists. Skipping creation.';
    RETURN;
END

PRINT 'Creating Masters_Suppliers table...';
GO

CREATE TABLE Masters_Suppliers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SupplierCode NVARCHAR(50) NOT NULL UNIQUE,
    SupplierName NVARCHAR(200) NOT NULL,

    -- Classification
    SupplierType NVARCHAR(50) NULL,
    Category NVARCHAR(100) NULL,

    -- Contact Information
    ContactPerson NVARCHAR(100) NULL,
    ContactNumber NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Website NVARCHAR(200) NULL,

    -- Address
    AddressLine1 NVARCHAR(200) NULL,
    AddressLine2 NVARCHAR(200) NULL,
    City NVARCHAR(100) NULL,
    State NVARCHAR(100) NULL,
    Country NVARCHAR(100) NULL DEFAULT 'India',
    PostalCode NVARCHAR(20) NULL,

    -- Tax Information
    GSTNumber NVARCHAR(20) NULL,
    PANNumber NVARCHAR(20) NULL,
    TaxStatus NVARCHAR(50) NULL,

    -- Payment Terms
    PaymentTerms NVARCHAR(100) NULL,
    CreditDays INT NULL,
    CreditLimit DECIMAL(18,2) NULL,

    -- Bank Details
    BankName NVARCHAR(100) NULL,
    BankAccountNumber NVARCHAR(50) NULL,
    IFSCCode NVARCHAR(20) NULL,
    BankBranch NVARCHAR(100) NULL,

    -- Performance Metrics
    OnTimeDeliveryRate DECIMAL(5,2) NULL,
    QualityRating DECIMAL(3,2) NULL,
    TotalOrders INT NULL DEFAULT 0,
    RejectedOrders INT NULL DEFAULT 0,

    -- Capabilities
    ServicesOffered NVARCHAR(500) NULL,
    ProcessCapabilities NVARCHAR(500) NULL,
    MaterialsSupplied NVARCHAR(500) NULL,

    -- Lead Times
    StandardLeadTimeDays INT NULL,
    MinimumOrderQuantity INT NULL,

    -- Status
    IsActive BIT NOT NULL DEFAULT 1,
    IsApproved BIT NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NULL DEFAULT 'Active',
    ApprovalStatus NVARCHAR(50) NULL DEFAULT 'Pending',

    -- Notes
    Remarks NVARCHAR(500) NULL,
    InternalNotes NVARCHAR(MAX) NULL,

    -- Audit
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL
);

GO

-- Create indexes for better performance
CREATE INDEX IX_Masters_Suppliers_SupplierCode ON Masters_Suppliers(SupplierCode);
CREATE INDEX IX_Masters_Suppliers_SupplierType ON Masters_Suppliers(SupplierType);
CREATE INDEX IX_Masters_Suppliers_Category ON Masters_Suppliers(Category);
CREATE INDEX IX_Masters_Suppliers_IsActive ON Masters_Suppliers(IsActive);
CREATE INDEX IX_Masters_Suppliers_IsApproved ON Masters_Suppliers(IsApproved);
CREATE INDEX IX_Masters_Suppliers_ApprovalStatus ON Masters_Suppliers(ApprovalStatus);

GO

PRINT 'Masters_Suppliers table created successfully!';
PRINT '';
PRINT '=== Table Structure ===';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Masters_Suppliers'
ORDER BY ORDINAL_POSITION;

GO
