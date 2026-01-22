-- =============================================
-- MultiHitech ERP Database - INT ID Migration
-- Drop and recreate database with INT IDENTITY instead of GUID
-- =============================================

USE master;
GO

-- Drop existing database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'MultiHitechERP')
BEGIN
    ALTER DATABASE MultiHitechERP SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MultiHitechERP;
END
GO

-- Create fresh database
CREATE DATABASE MultiHitechERP;
GO

USE MultiHitechERP;
GO

-- =============================================
-- MASTER TABLES
-- =============================================

-- Customers Table
CREATE TABLE Masters_Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerCode NVARCHAR(50) NOT NULL UNIQUE,
    CustomerName NVARCHAR(200) NOT NULL,
    CustomerType NVARCHAR(50) NOT NULL, -- 'Direct', 'Agent', 'Dealer'
    ContactPerson NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100),
    PinCode NVARCHAR(20),
    GSTNo NVARCHAR(50),
    PANNo NVARCHAR(50),
    CreditDays INT DEFAULT 0,
    CreditLimit DECIMAL(18,2) DEFAULT 0,
    PaymentTerms NVARCHAR(200),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100)
);
GO

-- Products Table (Rollers - Printing & Magnetic only)
CREATE TABLE Masters_Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PartCode NVARCHAR(50) NOT NULL UNIQUE,
    CustomerId INT,
    CustomerName NVARCHAR(200),
    ModelName NVARCHAR(200) NOT NULL,
    RollerType NVARCHAR(50) NOT NULL, -- 'Printing Roller', 'Magnetic Roller'
    Diameter DECIMAL(10,2),
    Length DECIMAL(10,2),
    Weight DECIMAL(10,3),
    MaterialGrade NVARCHAR(50),
    SurfaceFinish NVARCHAR(100),
    Hardness NVARCHAR(50),
    DrawingNo NVARCHAR(100),
    RevisionNo NVARCHAR(20),
    DrawingId INT,
    ProcessTemplateId INT,
    ProductTemplateId INT,
    StandardCost DECIMAL(18,2),
    SellingPrice DECIMAL(18,2),
    StandardLeadTimeDays INT,
    MinOrderQuantity INT,
    Category NVARCHAR(100),
    ProductType NVARCHAR(100),
    Description NVARCHAR(500),
    HSNCode NVARCHAR(20),
    UOM NVARCHAR(20) DEFAULT 'PCS',
    Remarks NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_Products_Customer FOREIGN KEY (CustomerId)
        REFERENCES Masters_Customers(Id),
    CONSTRAINT CHK_Products_Diameter CHECK (Diameter > 0 OR Diameter IS NULL),
    CONSTRAINT CHK_Products_Length CHECK (Length > 0 OR Length IS NULL),
    CONSTRAINT CHK_Products_Weight CHECK (Weight > 0 OR Weight IS NULL),
    CONSTRAINT CHK_Products_RollerType CHECK (RollerType IN ('Printing Roller', 'Magnetic Roller'))
);
GO

-- Raw Materials Table
CREATE TABLE Masters_RawMaterials (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MaterialCode NVARCHAR(50) NOT NULL UNIQUE,
    MaterialName NVARCHAR(200) NOT NULL,
    MaterialType NVARCHAR(50), -- 'Steel', 'Rubber', 'Chrome', 'Magnetic Material'
    Grade NVARCHAR(50),
    Specification NVARCHAR(200),
    UOM NVARCHAR(20) DEFAULT 'KG',
    StandardCost DECIMAL(18,2),
    MinStockLevel DECIMAL(18,2),
    MaxStockLevel DECIMAL(18,2),
    ReorderLevel DECIMAL(18,2),
    LeadTimeDays INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100)
);
GO

-- Machines Table
CREATE TABLE Masters_Machines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MachineCode NVARCHAR(50) NOT NULL UNIQUE,
    MachineName NVARCHAR(200) NOT NULL,
    MachineType NVARCHAR(100), -- 'Lathe', 'Grinding', 'Coating', 'Magnetizing'
    Manufacturer NVARCHAR(200),
    ModelNo NVARCHAR(100),
    SerialNo NVARCHAR(100),
    YearOfManufacture INT,
    Capacity NVARCHAR(100),
    Location NVARCHAR(200),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100)
);
GO

-- Process Master Table
CREATE TABLE Masters_Processes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProcessCode NVARCHAR(50) NOT NULL UNIQUE,
    ProcessName NVARCHAR(200) NOT NULL,
    ProcessType NVARCHAR(100), -- 'Machining', 'Coating', 'Magnetizing', 'Grinding'
    Description NVARCHAR(500),
    StandardTime INT, -- in minutes
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100)
);
GO

-- Drawings Table
CREATE TABLE Masters_Drawings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DrawingNo NVARCHAR(100) NOT NULL UNIQUE,
    DrawingName NVARCHAR(200) NOT NULL,
    RevisionNo NVARCHAR(20),
    RevisionDate DATETIME,
    CustomerId INT,
    ProductId INT,
    FilePath NVARCHAR(500),
    FileType NVARCHAR(50),
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_Drawings_Customer FOREIGN KEY (CustomerId)
        REFERENCES Masters_Customers(Id),
    CONSTRAINT FK_Drawings_Product FOREIGN KEY (ProductId)
        REFERENCES Masters_Products(Id)
);
GO

-- Add FK for DrawingId in Products
ALTER TABLE Masters_Products
ADD CONSTRAINT FK_Products_Drawing FOREIGN KEY (DrawingId)
    REFERENCES Masters_Drawings(Id);
GO

-- Process Templates Table
CREATE TABLE Masters_ProcessTemplates (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TemplateName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    ProductType NVARCHAR(100),
    TotalEstimatedTime INT, -- in minutes
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_ProcessTemplate_ProcessTemplate FOREIGN KEY (Id)
        REFERENCES Masters_ProcessTemplates(Id)
);
GO

-- Product Templates Table
CREATE TABLE Masters_ProductTemplates (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TemplateName NVARCHAR(200) NOT NULL,
    RollerType NVARCHAR(50),
    Description NVARCHAR(500),
    ProcessTemplateId INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_ProductTemplate_ProcessTemplate FOREIGN KEY (ProcessTemplateId)
        REFERENCES Masters_ProcessTemplates(Id)
);
GO

-- Add FKs for Templates in Products
ALTER TABLE Masters_Products
ADD CONSTRAINT FK_Products_ProcessTemplate FOREIGN KEY (ProcessTemplateId)
    REFERENCES Masters_ProcessTemplates(Id);
GO

ALTER TABLE Masters_Products
ADD CONSTRAINT FK_Products_ProductTemplate FOREIGN KEY (ProductTemplateId)
    REFERENCES Masters_ProductTemplates(Id);
GO

-- BOM (Bill of Materials) Table
CREATE TABLE Masters_BOM (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BOMNo NVARCHAR(50) NOT NULL UNIQUE,
    ProductId INT NOT NULL,
    ProductCode NVARCHAR(50),
    ProductName NVARCHAR(200),
    RevisionNumber INT DEFAULT 1,
    RevisionDate DATETIME,
    EffectiveDate DATETIME,
    ExpiryDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'Draft', -- Draft, Approved, Obsolete
    TotalCost DECIMAL(18,2),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_BOM_Product FOREIGN KEY (ProductId)
        REFERENCES Masters_Products(Id)
);
GO

-- BOM Items Table
CREATE TABLE Masters_BOMItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BOMId INT NOT NULL,
    ItemType NVARCHAR(50) NOT NULL, -- 'Material', 'ChildPart', 'Process'
    ItemId INT NOT NULL,
    ItemCode NVARCHAR(50),
    ItemName NVARCHAR(200),
    Quantity DECIMAL(18,3) NOT NULL,
    UOM NVARCHAR(20),
    UnitCost DECIMAL(18,2),
    TotalCost DECIMAL(18,2),
    ScrapPercentage DECIMAL(5,2) DEFAULT 0,
    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_BOMItems_BOM FOREIGN KEY (BOMId)
        REFERENCES Masters_BOM(Id) ON DELETE CASCADE
);
GO

-- Child Parts Table (sub-assemblies/components)
CREATE TABLE Masters_ChildParts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ChildPartCode NVARCHAR(50) NOT NULL UNIQUE,
    ChildPartName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    UOM NVARCHAR(20) DEFAULT 'PCS',
    StandardCost DECIMAL(18,2),
    LeadTimeDays INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100)
);
GO

-- =============================================
-- OPERATIONAL TABLES
-- =============================================

-- Orders Table
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderNo NVARCHAR(50) NOT NULL UNIQUE,
    OrderDate DATETIME NOT NULL,
    CustomerId INT NOT NULL,
    CustomerName NVARCHAR(200),
    ProductId INT NOT NULL,
    ProductName NVARCHAR(200),
    Quantity INT NOT NULL,
    UOM NVARCHAR(20),
    UnitPrice DECIMAL(18,2),
    TotalAmount DECIMAL(18,2),
    DeliveryDate DATETIME,
    Priority NVARCHAR(50) DEFAULT 'Normal', -- High, Normal, Low
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, InProduction, Completed, Cancelled
    DrawingId INT,
    SpecialInstructions NVARCHAR(1000),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_Orders_Customer FOREIGN KEY (CustomerId)
        REFERENCES Masters_Customers(Id),
    CONSTRAINT FK_Orders_Product FOREIGN KEY (ProductId)
        REFERENCES Masters_Products(Id),
    CONSTRAINT FK_Orders_Drawing FOREIGN KEY (DrawingId)
        REFERENCES Masters_Drawings(Id)
);
GO

-- Job Cards Table
CREATE TABLE Production_JobCards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JobCardNo NVARCHAR(50) NOT NULL UNIQUE,
    OrderId INT NOT NULL,
    OrderNo NVARCHAR(50),
    ProductId INT NOT NULL,
    ProductName NVARCHAR(200),
    Quantity INT NOT NULL,
    StartDate DATETIME,
    PlannedEndDate DATETIME,
    ActualEndDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'NotStarted', -- NotStarted, InProgress, Completed, OnHold
    Priority NVARCHAR(50),
    CurrentProcessId INT,
    CurrentMachineId INT,
    CompletedQuantity INT DEFAULT 0,
    RejectedQuantity INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_JobCards_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),
    CONSTRAINT FK_JobCards_Product FOREIGN KEY (ProductId)
        REFERENCES Masters_Products(Id),
    CONSTRAINT FK_JobCards_Process FOREIGN KEY (CurrentProcessId)
        REFERENCES Masters_Processes(Id),
    CONSTRAINT FK_JobCards_Machine FOREIGN KEY (CurrentMachineId)
        REFERENCES Masters_Machines(Id)
);
GO

-- Job Card Execution/Operations Table
CREATE TABLE Production_JobCardExecutions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JobCardId INT NOT NULL,
    ProcessId INT NOT NULL,
    ProcessName NVARCHAR(200),
    MachineId INT,
    OperatorId INT,
    SequenceNo INT,
    PlannedStartTime DATETIME,
    PlannedEndTime DATETIME,
    ActualStartTime DATETIME,
    ActualEndTime DATETIME,
    StandardTime INT, -- minutes
    ActualTime INT, -- minutes
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, InProgress, Completed, Skipped
    QuantityProduced INT DEFAULT 0,
    QuantityRejected INT DEFAULT 0,
    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_JobCardExecution_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Production_JobCards(Id) ON DELETE CASCADE,
    CONSTRAINT FK_JobCardExecution_Process FOREIGN KEY (ProcessId)
        REFERENCES Masters_Processes(Id),
    CONSTRAINT FK_JobCardExecution_Machine FOREIGN KEY (MachineId)
        REFERENCES Masters_Machines(Id)
);
GO

-- Quality Control Table
CREATE TABLE Quality_QCResults (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    QCNo NVARCHAR(50) NOT NULL UNIQUE,
    JobCardId INT NOT NULL,
    OrderId INT,
    ProductId INT NOT NULL,
    InspectionDate DATETIME NOT NULL,
    InspectorName NVARCHAR(100),
    QuantityInspected INT NOT NULL,
    QuantityAccepted INT DEFAULT 0,
    QuantityRejected INT DEFAULT 0,
    InspectionStage NVARCHAR(100), -- 'Incoming', 'In-Process', 'Final'
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Approved, Rejected
    Remarks NVARCHAR(1000),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_QC_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Production_JobCards(Id),
    CONSTRAINT FK_QC_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),
    CONSTRAINT FK_QC_Product FOREIGN KEY (ProductId)
        REFERENCES Masters_Products(Id)
);
GO

-- Inventory/Stock Table
CREATE TABLE Inventory_Stock (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ItemType NVARCHAR(50) NOT NULL, -- 'RawMaterial', 'Product', 'ChildPart'
    ItemId INT NOT NULL,
    ItemCode NVARCHAR(50),
    ItemName NVARCHAR(200),
    CurrentStock DECIMAL(18,3) DEFAULT 0,
    ReservedStock DECIMAL(18,3) DEFAULT 0,
    AvailableStock AS (CurrentStock - ReservedStock) PERSISTED,
    UOM NVARCHAR(20),
    Location NVARCHAR(100),
    MinStockLevel DECIMAL(18,3),
    MaxStockLevel DECIMAL(18,3),
    LastUpdated DATETIME DEFAULT GETUTCDATE(),
    UpdatedBy NVARCHAR(100),

    CONSTRAINT UQ_Inventory_Item UNIQUE (ItemType, ItemId, Location)
);
GO

-- Inventory Transactions Table
CREATE TABLE Inventory_Transactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransactionNo NVARCHAR(50) NOT NULL UNIQUE,
    TransactionDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    TransactionType NVARCHAR(50) NOT NULL, -- 'Receipt', 'Issue', 'Transfer', 'Adjustment'
    ItemType NVARCHAR(50) NOT NULL,
    ItemId INT NOT NULL,
    ItemCode NVARCHAR(50),
    ItemName NVARCHAR(200),
    Quantity DECIMAL(18,3) NOT NULL,
    UOM NVARCHAR(20),
    FromLocation NVARCHAR(100),
    ToLocation NVARCHAR(100),
    ReferenceType NVARCHAR(50), -- 'Order', 'JobCard', 'PO', 'MaterialRequisition'
    ReferenceId INT,
    ReferenceNo NVARCHAR(50),
    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100)
);
GO

-- Delivery Challan Table
CREATE TABLE Dispatch_DeliveryChallans (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ChallanNo NVARCHAR(50) NOT NULL UNIQUE,
    ChallanDate DATETIME NOT NULL,
    OrderId INT NOT NULL,
    OrderNo NVARCHAR(50),
    CustomerId INT NOT NULL,
    CustomerName NVARCHAR(200),
    DeliveryAddress NVARCHAR(1000),
    TransportMode NVARCHAR(100),
    VehicleNo NVARCHAR(50),
    DriverName NVARCHAR(100),
    DriverContact NVARCHAR(20),
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Dispatched, Delivered, Returned
    DispatchDate DATETIME,
    DeliveryDate DATETIME,
    ReceivedBy NVARCHAR(100),
    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_DeliveryChallan_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),
    CONSTRAINT FK_DeliveryChallan_Customer FOREIGN KEY (CustomerId)
        REFERENCES Masters_Customers(Id)
);
GO

-- Delivery Challan Items Table
CREATE TABLE Dispatch_DeliveryChallanItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ChallanId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductCode NVARCHAR(50),
    ProductName NVARCHAR(200),
    Quantity INT NOT NULL,
    UOM NVARCHAR(20),
    SerialNumbers NVARCHAR(MAX),
    Remarks NVARCHAR(500),

    CONSTRAINT FK_ChallanItems_Challan FOREIGN KEY (ChallanId)
        REFERENCES Dispatch_DeliveryChallans(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ChallanItems_Product FOREIGN KEY (ProductId)
        REFERENCES Masters_Products(Id)
);
GO

-- Material Requisition Table
CREATE TABLE Stores_MaterialRequisitions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RequisitionNo NVARCHAR(50) NOT NULL UNIQUE,
    RequisitionDate DATETIME NOT NULL,
    JobCardId INT,
    OrderId INT,
    RequestedBy NVARCHAR(100),
    Department NVARCHAR(100),
    RequiredDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, PartiallyIssued, Issued, Cancelled
    ApprovedBy NVARCHAR(100),
    ApprovalDate DATETIME,
    IssuedBy NVARCHAR(100),
    IssuedDate DATETIME,
    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_MaterialRequisition_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Production_JobCards(Id),
    CONSTRAINT FK_MaterialRequisition_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id)
);
GO

-- Material Requisition Items Table
CREATE TABLE Stores_MaterialRequisitionItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RequisitionId INT NOT NULL,
    MaterialId INT NOT NULL,
    MaterialCode NVARCHAR(50),
    MaterialName NVARCHAR(200),
    RequestedQuantity DECIMAL(18,3) NOT NULL,
    IssuedQuantity DECIMAL(18,3) DEFAULT 0,
    PendingQuantity AS (RequestedQuantity - IssuedQuantity) PERSISTED,
    UOM NVARCHAR(20),
    Purpose NVARCHAR(200),
    Remarks NVARCHAR(500),

    CONSTRAINT FK_RequisitionItems_Requisition FOREIGN KEY (RequisitionId)
        REFERENCES Stores_MaterialRequisitions(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RequisitionItems_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_RawMaterials(Id)
);
GO

-- Material Issues Table (Actual material issued from store)
CREATE TABLE Stores_MaterialIssues (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IssueNo NVARCHAR(50) NOT NULL UNIQUE,
    IssueDate DATETIME NOT NULL,
    RequisitionId INT,
    JobCardId INT,
    IssuedTo NVARCHAR(100),
    IssuedBy NVARCHAR(100),
    Status NVARCHAR(50) DEFAULT 'Issued',
    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_MaterialIssue_Requisition FOREIGN KEY (RequisitionId)
        REFERENCES Stores_MaterialRequisitions(Id),
    CONSTRAINT FK_MaterialIssue_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Production_JobCards(Id)
);
GO

-- Material Issue Items Table
CREATE TABLE Stores_MaterialIssueItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IssueId INT NOT NULL,
    MaterialId INT NOT NULL,
    MaterialCode NVARCHAR(50),
    MaterialName NVARCHAR(200),
    IssuedQuantity DECIMAL(18,3) NOT NULL,
    UOM NVARCHAR(20),
    BatchNo NVARCHAR(50),
    Remarks NVARCHAR(500),

    CONSTRAINT FK_IssueItems_Issue FOREIGN KEY (IssueId)
        REFERENCES Stores_MaterialIssues(Id) ON DELETE CASCADE,
    CONSTRAINT FK_IssueItems_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_RawMaterials(Id)
);
GO

-- Material Pieces/Cuts Tracking Table
CREATE TABLE Stores_MaterialPieces (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PieceNo NVARCHAR(50) NOT NULL UNIQUE,
    MaterialId INT NOT NULL,
    MaterialCode NVARCHAR(50),
    MaterialName NVARCHAR(200),
    OriginalLength DECIMAL(18,3),
    CurrentLength DECIMAL(18,3),
    Width DECIMAL(18,3),
    Thickness DECIMAL(18,3),
    Weight DECIMAL(18,3),
    UOM NVARCHAR(20),
    Location NVARCHAR(100),
    BatchNo NVARCHAR(50),
    Status NVARCHAR(50) DEFAULT 'Available', -- Available, InUse, Consumed, Scrap
    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_MaterialPiece_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_RawMaterials(Id)
);
GO

PRINT 'Database schema created successfully with INT IDENTITY IDs!';
GO
