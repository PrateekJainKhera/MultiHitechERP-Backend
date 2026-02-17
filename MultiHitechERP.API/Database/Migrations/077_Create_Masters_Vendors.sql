-- Migration 077: Create Vendor Master Table
-- Vendors are used in Purchase Requests/Orders (procurement)
-- Separate from Suppliers (which is used for OSP/outsourcing)

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Masters_Vendors')
BEGIN
    CREATE TABLE Masters_Vendors (
        Id              INT PRIMARY KEY IDENTITY(1,1),
        VendorCode      NVARCHAR(20)  NOT NULL,
        VendorName      NVARCHAR(200) NOT NULL,
        VendorType      NVARCHAR(50)  NOT NULL DEFAULT 'Trader',
        ContactPerson   NVARCHAR(100) NULL,
        Email           NVARCHAR(200) NULL,
        Phone           NVARCHAR(20)  NULL,
        Address         NVARCHAR(500) NULL,
        City            NVARCHAR(100) NULL,
        State           NVARCHAR(100) NULL,
        Country         NVARCHAR(100) NOT NULL DEFAULT 'India',
        PinCode         NVARCHAR(10)  NULL,
        GSTNo           NVARCHAR(15)  NULL,
        PANNo           NVARCHAR(10)  NULL,
        CreditDays      INT           NULL,
        CreditLimit     DECIMAL(18,2) NULL,
        PaymentTerms    NVARCHAR(100) NULL DEFAULT 'Net 30 Days',
        IsActive        BIT           NOT NULL DEFAULT 1,
        CreatedAt       DATETIME2     NOT NULL DEFAULT GETDATE(),
        CreatedBy       NVARCHAR(100) NULL,
        UpdatedAt       DATETIME2     NOT NULL DEFAULT GETDATE(),
        UpdatedBy       NVARCHAR(100) NULL,
        CONSTRAINT UQ_Vendors_VendorCode UNIQUE (VendorCode)
    );

    PRINT 'Masters_Vendors table created successfully.';
END
ELSE
BEGIN
    PRINT 'Masters_Vendors table already exists. Skipping.';
END
