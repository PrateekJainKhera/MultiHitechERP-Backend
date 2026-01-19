-- =====================================================================
-- MultiHitech ERP - Phase 1 Database Schema
-- SQL Server 2022
-- Date: January 2026
-- =====================================================================

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MultiHitechERP')
BEGIN
    CREATE DATABASE MultiHitechERP;
END
GO

USE MultiHitechERP;
GO

-- =====================================================================
-- MASTERS SCHEMA
-- =====================================================================

-- Customers Table
CREATE TABLE Masters_Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerCode NVARCHAR(50) NOT NULL UNIQUE,
    CustomerName NVARCHAR(200) NOT NULL,
    CustomerType NVARCHAR(20) NOT NULL, -- 'Direct', 'Agent', 'Dealer'
    ContactPerson NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100) DEFAULT 'India',
    PinCode NVARCHAR(10),
    GSTNo NVARCHAR(20),
    PANNo NVARCHAR(20),
    CreditDays INT DEFAULT 0,
    CreditLimit DECIMAL(18,2) DEFAULT 0,
    PaymentTerms NVARCHAR(200),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT CHK_Customers_Type CHECK (CustomerType IN ('Direct', 'Agent', 'Dealer')),
    CONSTRAINT CHK_Customers_CreditDays CHECK (CreditDays >= 0)
);

CREATE INDEX IX_Customers_Code ON Masters_Customers(CustomerCode);
CREATE INDEX IX_Customers_Active ON Masters_Customers(IsActive);
GO

-- Process Templates Table (must be created before Products)
CREATE TABLE Masters_ProcessTemplates (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TemplateName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    TotalEstimatedTimeMin INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100)
);
GO

-- Product Templates Table (must be created before Products)
CREATE TABLE Masters_ProductTemplates (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TemplateName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    ProductType NVARCHAR(100),
    ProcessTemplateId UNIQUEIDENTIFIER,
    EstimatedLeadTimeDays INT,
    StandardCost DECIMAL(18,2),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_ProductTemplate_ProcessTemplate FOREIGN KEY (ProcessTemplateId)
        REFERENCES Masters_ProcessTemplates(Id)
);
GO

-- Products Table
CREATE TABLE Masters_Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PartCode NVARCHAR(50) NOT NULL,
    CustomerId UNIQUEIDENTIFIER,
    CustomerName NVARCHAR(200),
    ModelName NVARCHAR(200) NOT NULL,
    RollerType NVARCHAR(50) NOT NULL,
    Diameter DECIMAL(10,2),
    Length DECIMAL(10,2),
    MaterialGrade NVARCHAR(50),
    SurfaceFinish NVARCHAR(100),
    Hardness NVARCHAR(50),
    DrawingNo NVARCHAR(100),
    RevisionNo NVARCHAR(20),
    ProcessTemplateId UNIQUEIDENTIFIER,
    ProductTemplateId UNIQUEIDENTIFIER,
    StandardCost DECIMAL(18,2),
    StandardLeadTimeDays INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),

    CONSTRAINT FK_Products_Customer FOREIGN KEY (CustomerId)
        REFERENCES Masters_Customers(Id),
    CONSTRAINT FK_Products_ProcessTemplate FOREIGN KEY (ProcessTemplateId)
        REFERENCES Masters_ProcessTemplates(Id),
    CONSTRAINT FK_Products_ProductTemplate FOREIGN KEY (ProductTemplateId)
        REFERENCES Masters_ProductTemplates(Id),

    CONSTRAINT CHK_Products_Diameter CHECK (Diameter > 0 OR Diameter IS NULL),
    CONSTRAINT CHK_Products_Length CHECK (Length > 0 OR Length IS NULL),
    CONSTRAINT UQ_Products_PartCode_Customer UNIQUE (PartCode, CustomerId)
);

CREATE INDEX IX_Products_Customer ON Masters_Products(CustomerId);
CREATE INDEX IX_Products_PartCode ON Masters_Products(PartCode);
CREATE INDEX IX_Products_Active ON Masters_Products(IsActive);
GO

-- Child Part Templates Table
CREATE TABLE Masters_ChildPartTemplates (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TemplateName NVARCHAR(200) NOT NULL,
    ChildPartType NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    Length DECIMAL(10,2),
    Diameter DECIMAL(10,2),
    InnerDiameter DECIMAL(10,2),
    OuterDiameter DECIMAL(10,2),
    Thickness DECIMAL(10,2),
    ProcessTemplateId UNIQUEIDENTIFIER,
    EstimatedCost DECIMAL(18,2),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_ChildPartTemplate_ProcessTemplate FOREIGN KEY (ProcessTemplateId)
        REFERENCES Masters_ProcessTemplates(Id)
);
GO

-- Raw Materials Table
CREATE TABLE Masters_RawMaterials (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaterialCode NVARCHAR(50) NOT NULL UNIQUE,
    MaterialName NVARCHAR(200) NOT NULL,
    Grade NVARCHAR(50),
    Shape NVARCHAR(20),
    Diameter DECIMAL(10,2),
    StandardLengthMM DECIMAL(10,2),
    Density DECIMAL(10,4),
    UnitOfMeasure NVARCHAR(20) DEFAULT 'kg',
    UnitPrice DECIMAL(18,2),
    MinStockLevel DECIMAL(18,2),
    MaxStockLevel DECIMAL(18,2),
    ReorderQuantity DECIMAL(18,2),
    StorageLocation NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT CHK_RawMaterials_Density CHECK (Density > 0 OR Density IS NULL),
    CONSTRAINT CHK_RawMaterials_UnitPrice CHECK (UnitPrice >= 0 OR UnitPrice IS NULL)
);

CREATE INDEX IX_RawMaterials_Code ON Masters_RawMaterials(MaterialCode);
CREATE INDEX IX_RawMaterials_Grade ON Masters_RawMaterials(Grade);
GO

-- Machines Table (must be created before Processes)
CREATE TABLE Masters_Machines (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MachineCode NVARCHAR(50) NOT NULL UNIQUE,
    MachineName NVARCHAR(200) NOT NULL,
    Type NVARCHAR(50),
    Manufacturer NVARCHAR(100),
    Model NVARCHAR(100),
    YearOfManufacture INT,
    Capacity NVARCHAR(200),
    MaxDiameter DECIMAL(10,2),
    MaxLength DECIMAL(10,2),
    Status NVARCHAR(20) DEFAULT 'Active',
    CurrentOperatorId UNIQUEIDENTIFIER,
    CurrentJobCardId UNIQUEIDENTIFIER,
    ShopFloorLocation NVARCHAR(100),
    Department NVARCHAR(50),
    LastMaintenanceDate DATE,
    NextMaintenanceDate DATE,
    MaintenanceIntervalDays INT,
    AvailableHoursPerShift DECIMAL(5,2) DEFAULT 8.0,
    NumberOfShifts INT DEFAULT 1,
    TargetUtilizationPercent DECIMAL(5,2) DEFAULT 85.0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT CHK_Machines_Status CHECK (Status IN ('Active', 'In_Use', 'Idle', 'Maintenance', 'Breakdown', 'Retired'))
);

CREATE INDEX IX_Machines_Code ON Masters_Machines(MachineCode);
CREATE INDEX IX_Machines_Type ON Masters_Machines(Type);
CREATE INDEX IX_Machines_Status ON Masters_Machines(Status);
GO

-- Processes Table
CREATE TABLE Masters_Processes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProcessCode NVARCHAR(50) NOT NULL UNIQUE,
    ProcessName NVARCHAR(200) NOT NULL,
    Category NVARCHAR(50),
    SubCategory NVARCHAR(50),
    StandardTimeMin INT,
    SetupTimeMin INT,
    CycleTimeMin INT,
    MachineType NVARCHAR(50),
    DefaultMachineId UNIQUEIDENTIFIER,
    IsOutsourced BIT DEFAULT 0,
    OSPVendorId UNIQUEIDENTIFIER,
    OSPLeadTimeDays INT,
    OSPCostPerPiece DECIMAL(18,2),
    RequiresInspection BIT DEFAULT 0,
    InspectionType NVARCHAR(100),
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_Processes_DefaultMachine FOREIGN KEY (DefaultMachineId)
        REFERENCES Masters_Machines(Id),

    CONSTRAINT CHK_Processes_StandardTime CHECK (StandardTimeMin >= 0 OR StandardTimeMin IS NULL),
    CONSTRAINT CHK_Processes_SetupTime CHECK (SetupTimeMin >= 0 OR SetupTimeMin IS NULL)
);

CREATE INDEX IX_Processes_Code ON Masters_Processes(ProcessCode);
CREATE INDEX IX_Processes_Category ON Masters_Processes(Category);
CREATE INDEX IX_Processes_Outsourced ON Masters_Processes(IsOutsourced);
GO

-- Process Template Steps
CREATE TABLE Masters_ProcessTemplateSteps (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProcessTemplateId UNIQUEIDENTIFIER NOT NULL,
    StepNo INT NOT NULL,
    ProcessId UNIQUEIDENTIFIER NOT NULL,
    ProcessName NVARCHAR(200),
    IsMandatory BIT DEFAULT 1,
    EstimatedTimeMin INT,
    QualityCheckRequired BIT DEFAULT 0,

    CONSTRAINT FK_ProcessTemplateSteps_Template FOREIGN KEY (ProcessTemplateId)
        REFERENCES Masters_ProcessTemplates(Id),
    CONSTRAINT FK_ProcessTemplateSteps_Process FOREIGN KEY (ProcessId)
        REFERENCES Masters_Processes(Id),

    CONSTRAINT UQ_ProcessTemplateSteps_StepNo UNIQUE (ProcessTemplateId, StepNo),
    CONSTRAINT CHK_ProcessTemplateSteps_StepNo CHECK (StepNo > 0)
);
GO

-- Drawings Table
CREATE TABLE Masters_Drawings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DrawingNumber NVARCHAR(100) NOT NULL,
    DrawingName NVARCHAR(200) NOT NULL,
    Revision NVARCHAR(20) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Draft',
    PartType NVARCHAR(50),
    LinkedPartName NVARCHAR(200),
    LinkedProductId UNIQUEIDENTIFIER,
    LinkedCustomerId UNIQUEIDENTIFIER,
    FileName NVARCHAR(255),
    FileExtension NVARCHAR(10),
    FileSizeKB INT,
    FileStoragePath NVARCHAR(500),
    UploadDate DATETIME DEFAULT GETUTCDATE(),
    UploadedBy NVARCHAR(100),
    ApprovedBy NVARCHAR(100),
    ApprovedDate DATETIME,
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,

    CONSTRAINT UQ_Drawings_Number_Revision UNIQUE (DrawingNumber, Revision),
    CONSTRAINT CHK_Drawings_Status CHECK (Status IN ('Draft', 'Approved', 'Obsolete'))
);

CREATE INDEX IX_Drawings_Number ON Masters_Drawings(DrawingNumber);
CREATE INDEX IX_Drawings_PartType ON Masters_Drawings(PartType);
CREATE INDEX IX_Drawings_Status ON Masters_Drawings(Status);
GO

-- =====================================================================
-- ORDERS SCHEMA
-- =====================================================================

CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OrderNo NVARCHAR(50) NOT NULL UNIQUE,
    OrderDate DATETIME NOT NULL,
    DueDate DATETIME NOT NULL,
    AdjustedDueDate DATETIME,

    -- Customer & Product
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,

    -- Quantities
    Quantity INT NOT NULL,
    OriginalQuantity INT NOT NULL,
    QtyCompleted INT DEFAULT 0,
    QtyRejected INT DEFAULT 0,
    QtyInProgress INT DEFAULT 0,
    QtyScrap INT DEFAULT 0,

    -- Status
    Status NVARCHAR(20) DEFAULT 'Pending',
    Priority NVARCHAR(20) DEFAULT 'Medium',
    PlanningStatus NVARCHAR(20) DEFAULT 'Not Planned',

    -- Drawing Review (GATE)
    DrawingReviewStatus NVARCHAR(20) DEFAULT 'Pending',
    DrawingReviewedBy NVARCHAR(100),
    DrawingReviewedAt DATETIME,
    DrawingReviewNotes NVARCHAR(1000),

    -- Production Tracking
    CurrentProcess NVARCHAR(200),
    CurrentMachine NVARCHAR(100),
    CurrentOperator NVARCHAR(100),
    ProductionStartDate DATETIME,
    ProductionEndDate DATETIME,

    -- Rescheduling
    DelayReason NVARCHAR(100),
    RescheduleCount INT DEFAULT 0,

    -- Material
    MaterialGradeApproved BIT DEFAULT 0,
    MaterialGradeApprovalDate DATETIME,
    MaterialGradeApprovedBy NVARCHAR(100),

    -- Financial
    OrderValue DECIMAL(18,2),
    AdvancePayment DECIMAL(18,2),
    BalancePayment DECIMAL(18,2),

    -- Audit
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),
    Version INT DEFAULT 1,

    CONSTRAINT FK_Orders_Customer FOREIGN KEY (CustomerId)
        REFERENCES Masters_Customers(Id),
    CONSTRAINT FK_Orders_Product FOREIGN KEY (ProductId)
        REFERENCES Masters_Products(Id),

    CONSTRAINT CHK_Orders_Quantity CHECK (Quantity > 0),
    CONSTRAINT CHK_Orders_DueDate CHECK (DueDate >= OrderDate),
    CONSTRAINT CHK_Orders_Status CHECK (Status IN ('Pending', 'In Progress', 'Completed', 'Rejected', 'On Hold', 'Cancelled')),
    CONSTRAINT CHK_Orders_Priority CHECK (Priority IN ('Low', 'Medium', 'High', 'Urgent')),
    CONSTRAINT CHK_Orders_PlanningStatus CHECK (PlanningStatus IN ('Not Planned', 'Planned', 'Released')),
    CONSTRAINT CHK_Orders_DrawingReviewStatus CHECK (DrawingReviewStatus IN ('Pending', 'In Review', 'Approved', 'Needs Revision')),
    CONSTRAINT CHK_Orders_QtyReconciliation CHECK (QtyCompleted + QtyInProgress + QtyRejected <= OriginalQuantity)
);

CREATE INDEX IX_Orders_OrderNo ON Orders(OrderNo);
CREATE INDEX IX_Orders_Customer ON Orders(CustomerId);
CREATE INDEX IX_Orders_Product ON Orders(ProductId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_DrawingReviewStatus ON Orders(DrawingReviewStatus);
CREATE INDEX IX_Orders_PlanningStatus ON Orders(PlanningStatus);
CREATE INDEX IX_Orders_DueDate ON Orders(DueDate);
GO

-- =====================================================================
-- DRAWING REVIEW SCHEMA
-- =====================================================================

-- Drawing reviews are tracked in Orders.DrawingReviewStatus
-- Additional detailed review history can be stored here
CREATE TABLE DrawingReviews (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OrderId UNIQUEIDENTIFIER NOT NULL,
    DrawingId UNIQUEIDENTIFIER,
    ReviewStatus NVARCHAR(20) NOT NULL,
    ReviewedBy NVARCHAR(100),
    ReviewedAt DATETIME DEFAULT GETUTCDATE(),
    Notes NVARCHAR(1000),
    RevisionRequested BIT DEFAULT 0,
    RevisionNotes NVARCHAR(1000),

    CONSTRAINT FK_DrawingReviews_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),
    CONSTRAINT FK_DrawingReviews_Drawing FOREIGN KEY (DrawingId)
        REFERENCES Masters_Drawings(Id)
);

CREATE INDEX IX_DrawingReviews_Order ON DrawingReviews(OrderId);
GO

-- =====================================================================
-- PLANNING SCHEMA (JOB CARDS)
-- =====================================================================

CREATE TABLE Planning_JobCards (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    JobCardNo NVARCHAR(50) NOT NULL UNIQUE,
    CreationType NVARCHAR(20) DEFAULT 'Auto-Generated',

    -- Order Reference
    OrderId UNIQUEIDENTIFIER NOT NULL,
    OrderNo NVARCHAR(50),

    -- Drawing (REQUIRED)
    DrawingId UNIQUEIDENTIFIER,
    DrawingNumber NVARCHAR(100),
    DrawingRevision NVARCHAR(20),
    DrawingSelectionType NVARCHAR(10) DEFAULT 'auto',

    -- Child Part
    ChildPartId UNIQUEIDENTIFIER,
    ChildPartName NVARCHAR(200),
    ChildPartTemplateId UNIQUEIDENTIFIER,

    -- Process
    ProcessId UNIQUEIDENTIFIER NOT NULL,
    ProcessName NVARCHAR(200),
    StepNo INT,
    ProcessTemplateId UNIQUEIDENTIFIER,

    -- Quantities
    Quantity INT NOT NULL,
    CompletedQty INT DEFAULT 0,
    RejectedQty INT DEFAULT 0,
    ReworkQty INT DEFAULT 0,
    InProgressQty INT DEFAULT 0,

    -- Status
    Status NVARCHAR(20) DEFAULT 'Pending',

    -- Machine & Operator Assignment
    AssignedMachineId UNIQUEIDENTIFIER,
    AssignedMachineName NVARCHAR(100),
    AssignedOperatorId UNIQUEIDENTIFIER,
    AssignedOperatorName NVARCHAR(100),

    -- Time Tracking
    EstimatedSetupTimeMin INT,
    EstimatedCycleTimeMin INT,
    EstimatedTotalTimeMin INT,
    ActualStartTime DATETIME,
    ActualEndTime DATETIME,
    ActualTimeMin INT,

    -- Material
    MaterialStatus NVARCHAR(20) DEFAULT 'Pending',
    MaterialStatusUpdatedAt DATETIME,

    -- Manufacturing Dimensions (JSON)
    ManufacturingDimensions NVARCHAR(MAX),

    -- Priority
    Priority NVARCHAR(20) DEFAULT 'Medium',

    -- Scheduling
    ScheduleStatus NVARCHAR(20) DEFAULT 'Not Scheduled',
    ScheduledStartDate DATETIME,
    ScheduledEndDate DATETIME,

    -- Rework
    IsRework BIT DEFAULT 0,
    ReworkOrderId UNIQUEIDENTIFIER,
    ParentJobCardId UNIQUEIDENTIFIER,

    -- Audit
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100),
    Version INT DEFAULT 1,

    CONSTRAINT FK_JobCards_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),
    CONSTRAINT FK_JobCards_Process FOREIGN KEY (ProcessId)
        REFERENCES Masters_Processes(Id),
    CONSTRAINT FK_JobCards_Drawing FOREIGN KEY (DrawingId)
        REFERENCES Masters_Drawings(Id),
    CONSTRAINT FK_JobCards_Machine FOREIGN KEY (AssignedMachineId)
        REFERENCES Masters_Machines(Id),
    CONSTRAINT FK_JobCards_ParentJobCard FOREIGN KEY (ParentJobCardId)
        REFERENCES Planning_JobCards(Id),

    CONSTRAINT CHK_JobCards_Quantity CHECK (Quantity > 0),
    CONSTRAINT CHK_JobCards_Status CHECK (Status IN ('Pending', 'Pending Material', 'Ready', 'In Progress', 'Paused', 'Completed', 'Blocked')),
    CONSTRAINT CHK_JobCards_MaterialStatus CHECK (MaterialStatus IN ('Available', 'Pending', 'Partial')),
    CONSTRAINT CHK_JobCards_QtyReconciliation CHECK (CompletedQty + InProgressQty + RejectedQty <= Quantity)
);

CREATE INDEX IX_JobCards_JobCardNo ON Planning_JobCards(JobCardNo);
CREATE INDEX IX_JobCards_Order ON Planning_JobCards(OrderId);
CREATE INDEX IX_JobCards_ChildPart ON Planning_JobCards(ChildPartId);
CREATE INDEX IX_JobCards_Status ON Planning_JobCards(Status);
CREATE INDEX IX_JobCards_MaterialStatus ON Planning_JobCards(MaterialStatus);
CREATE INDEX IX_JobCards_Machine ON Planning_JobCards(AssignedMachineId);
CREATE INDEX IX_JobCards_Operator ON Planning_JobCards(AssignedOperatorId);
GO

-- Job Card Dependencies
CREATE TABLE Planning_JobCardDependencies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    JobCardId UNIQUEIDENTIFIER NOT NULL,
    DependsOnJobCardId UNIQUEIDENTIFIER NOT NULL,
    DependencyType NVARCHAR(20) DEFAULT 'Sequential',

    CONSTRAINT FK_Dependencies_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Planning_JobCards(Id),
    CONSTRAINT FK_Dependencies_DependsOn FOREIGN KEY (DependsOnJobCardId)
        REFERENCES Planning_JobCards(Id),

    CONSTRAINT UQ_Dependencies UNIQUE (JobCardId, DependsOnJobCardId)
);

CREATE INDEX IX_Dependencies_JobCard ON Planning_JobCardDependencies(JobCardId);
CREATE INDEX IX_Dependencies_DependsOn ON Planning_JobCardDependencies(DependsOnJobCardId);
GO

-- =====================================================================
-- INVENTORY SCHEMA
-- =====================================================================

CREATE TABLE Inventory_MaterialPieces (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaterialId UNIQUEIDENTIFIER NOT NULL,
    PieceNo NVARCHAR(50) NOT NULL UNIQUE,

    -- Length Tracking (CRITICAL)
    OriginalLengthMM DECIMAL(10,2) NOT NULL,
    CurrentLengthMM DECIMAL(10,2) NOT NULL,
    OriginalWeightKG DECIMAL(10,4) NOT NULL,
    CurrentWeightKG DECIMAL(10,4) NOT NULL,

    -- Status
    Status NVARCHAR(20) DEFAULT 'Available',
    AllocatedToRequisitionId UNIQUEIDENTIFIER,
    IssuedToJobCardId UNIQUEIDENTIFIER,

    -- Location
    StorageLocation NVARCHAR(100),
    BinNumber NVARCHAR(50),
    RackNumber NVARCHAR(50),

    -- GRN
    GRNNo NVARCHAR(50),
    ReceivedDate DATETIME DEFAULT GETUTCDATE(),
    SupplierBatchNo NVARCHAR(100),
    SupplierId UNIQUEIDENTIFIER,
    UnitCost DECIMAL(18,2),

    -- Audit
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME,

    CONSTRAINT FK_MaterialPieces_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_RawMaterials(Id),

    CONSTRAINT CHK_MaterialPieces_Length CHECK (CurrentLengthMM >= 0 AND CurrentLengthMM <= OriginalLengthMM),
    CONSTRAINT CHK_MaterialPieces_Weight CHECK (CurrentWeightKG >= 0 AND CurrentWeightKG <= OriginalWeightKG),
    CONSTRAINT CHK_MaterialPieces_Status CHECK (Status IN ('Available', 'Allocated', 'Issued', 'Scrap', 'Consumed'))
);

CREATE INDEX IX_MaterialPieces_Material ON Inventory_MaterialPieces(MaterialId);
CREATE INDEX IX_MaterialPieces_Status ON Inventory_MaterialPieces(Status);
CREATE INDEX IX_MaterialPieces_Location ON Inventory_MaterialPieces(StorageLocation);
CREATE INDEX IX_MaterialPieces_GRN ON Inventory_MaterialPieces(GRNNo);
GO

-- =====================================================================
-- STORES SCHEMA
-- =====================================================================

CREATE TABLE Stores_MaterialRequisitions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequisitionNo NVARCHAR(50) NOT NULL UNIQUE,
    RequisitionDate DATETIME DEFAULT GETUTCDATE(),

    -- Reference
    JobCardId UNIQUEIDENTIFIER,
    JobCardNo NVARCHAR(50),
    OrderId UNIQUEIDENTIFIER,
    OrderNo NVARCHAR(50),
    CustomerName NVARCHAR(200),

    -- Status
    Status NVARCHAR(20) DEFAULT 'Pending',
    Priority NVARCHAR(20) DEFAULT 'Medium',
    DueDate DATETIME,

    -- Approval
    RequestedBy NVARCHAR(100),
    ApprovedBy NVARCHAR(100),
    ApprovalDate DATETIME,

    Remarks NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_MaterialRequisitions_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Planning_JobCards(Id),
    CONSTRAINT FK_MaterialRequisitions_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),

    CONSTRAINT CHK_Requisitions_Status CHECK (Status IN ('Pending', 'Partial', 'Allocated', 'Issued', 'Cancelled'))
);

CREATE INDEX IX_Requisitions_RequisitionNo ON Stores_MaterialRequisitions(RequisitionNo);
CREATE INDEX IX_Requisitions_JobCard ON Stores_MaterialRequisitions(JobCardId);
CREATE INDEX IX_Requisitions_Status ON Stores_MaterialRequisitions(Status);
GO

CREATE TABLE Stores_MaterialRequisitionItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequisitionId UNIQUEIDENTIFIER NOT NULL,
    MaterialId UNIQUEIDENTIFIER NOT NULL,
    MaterialName NVARCHAR(200),
    MaterialGrade NVARCHAR(50),

    -- Dimensions
    Diameter DECIMAL(10,2),
    RequiredLengthMM DECIMAL(10,2),

    -- Quantities
    QuantityRequired DECIMAL(18,4) NOT NULL,
    QuantityAllocated DECIMAL(18,4) DEFAULT 0,
    QuantityIssued DECIMAL(18,4) DEFAULT 0,
    Unit NVARCHAR(20),

    -- Wastage
    ExpectedWastagePercent DECIMAL(5,2) DEFAULT 0,
    ExpectedWastage DECIMAL(18,4),

    CONSTRAINT FK_RequisitionItems_Requisition FOREIGN KEY (RequisitionId)
        REFERENCES Stores_MaterialRequisitions(Id),
    CONSTRAINT FK_RequisitionItems_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_RawMaterials(Id),

    CONSTRAINT CHK_RequisitionItems_Qty CHECK (QuantityRequired > 0),
    CONSTRAINT CHK_RequisitionItems_Allocated CHECK (QuantityAllocated <= QuantityRequired * (1 + ExpectedWastagePercent/100))
);
GO

CREATE TABLE Stores_MaterialAllocations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequisitionId UNIQUEIDENTIFIER NOT NULL,
    RequisitionItemId UNIQUEIDENTIFIER NOT NULL,
    MaterialPieceId UNIQUEIDENTIFIER NOT NULL,

    AllocatedLengthMM DECIMAL(10,2),
    AllocatedWeightKG DECIMAL(10,4),

    AllocationDate DATETIME DEFAULT GETUTCDATE(),
    AllocatedBy NVARCHAR(100),

    CONSTRAINT FK_Allocations_Requisition FOREIGN KEY (RequisitionId)
        REFERENCES Stores_MaterialRequisitions(Id),
    CONSTRAINT FK_Allocations_RequisitionItem FOREIGN KEY (RequisitionItemId)
        REFERENCES Stores_MaterialRequisitionItems(Id),
    CONSTRAINT FK_Allocations_MaterialPiece FOREIGN KEY (MaterialPieceId)
        REFERENCES Inventory_MaterialPieces(Id),

    CONSTRAINT CHK_Allocations_Length CHECK (AllocatedLengthMM > 0),
    CONSTRAINT CHK_Allocations_Weight CHECK (AllocatedWeightKG > 0)
);
GO

CREATE TABLE Stores_MaterialIssues (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IssueNo NVARCHAR(50) NOT NULL UNIQUE,
    IssueDate DATETIME DEFAULT GETUTCDATE(),

    RequisitionId UNIQUEIDENTIFIER NOT NULL,
    JobCardNo NVARCHAR(50),
    OrderNo NVARCHAR(50),

    MaterialName NVARCHAR(200),
    MaterialGrade NVARCHAR(50),

    TotalPieces INT,
    TotalIssuedLengthMM DECIMAL(10,2),
    TotalIssuedWeightKG DECIMAL(10,4),

    Status NVARCHAR(20) DEFAULT 'Issued',

    IssuedById NVARCHAR(100),
    IssuedByName NVARCHAR(100),
    ReceivedById NVARCHAR(100),
    ReceivedByName NVARCHAR(100),

    CONSTRAINT FK_MaterialIssues_Requisition FOREIGN KEY (RequisitionId)
        REFERENCES Stores_MaterialRequisitions(Id),

    CONSTRAINT CHK_Issues_Status CHECK (Status IN ('Pending', 'Issued', 'Returned', 'Cancelled'))
);

CREATE INDEX IX_MaterialIssues_IssueNo ON Stores_MaterialIssues(IssueNo);
CREATE INDEX IX_MaterialIssues_Requisition ON Stores_MaterialIssues(RequisitionId);
GO

CREATE TABLE Stores_MaterialIssueItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IssueId UNIQUEIDENTIFIER NOT NULL,
    MaterialPieceId UNIQUEIDENTIFIER NOT NULL,
    LengthIssuedMM DECIMAL(10,2),
    WeightIssuedKG DECIMAL(10,4),

    CONSTRAINT FK_IssueItems_Issue FOREIGN KEY (IssueId)
        REFERENCES Stores_MaterialIssues(Id),
    CONSTRAINT FK_IssueItems_MaterialPiece FOREIGN KEY (MaterialPieceId)
        REFERENCES Inventory_MaterialPieces(Id)
);
GO

-- =====================================================================
-- PRODUCTION SCHEMA
-- =====================================================================

CREATE TABLE Production_JobCardExecutions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    JobCardId UNIQUEIDENTIFIER NOT NULL,
    JobCardNo NVARCHAR(50),
    OrderNo NVARCHAR(50),

    -- Machine & Operator
    MachineId UNIQUEIDENTIFIER,
    MachineName NVARCHAR(100),
    OperatorId UNIQUEIDENTIFIER,
    OperatorName NVARCHAR(100),

    -- Time Tracking
    StartTime DATETIME NOT NULL,
    EndTime DATETIME,
    PausedTime DATETIME,
    ResumedTime DATETIME,
    TotalTimeMin INT,
    IdleTimeMin INT,

    -- Quantity Tracking
    QuantityStarted INT,
    QuantityCompleted INT,
    QuantityRejected INT,
    QuantityInProgress INT,

    -- Status
    ExecutionStatus NVARCHAR(20) DEFAULT 'Started',

    -- Notes
    Notes NVARCHAR(500),
    IssuesEncountered NVARCHAR(1000),

    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_Executions_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Planning_JobCards(Id),
    CONSTRAINT FK_Executions_Machine FOREIGN KEY (MachineId)
        REFERENCES Masters_Machines(Id),

    CONSTRAINT CHK_Executions_Status CHECK (ExecutionStatus IN ('Started', 'Paused', 'Resumed', 'Completed', 'Cancelled'))
);

CREATE INDEX IX_Executions_JobCard ON Production_JobCardExecutions(JobCardId);
CREATE INDEX IX_Executions_Machine ON Production_JobCardExecutions(MachineId);
GO

-- =====================================================================
-- QUALITY SCHEMA
-- =====================================================================

CREATE TABLE Quality_QCResults (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    JobCardId UNIQUEIDENTIFIER NOT NULL,
    JobCardNo NVARCHAR(50),
    OrderNo NVARCHAR(50),

    -- Inspection Details
    InspectionType NVARCHAR(50) NOT NULL,
    InspectedBy NVARCHAR(100) NOT NULL,
    InspectionDate DATETIME DEFAULT GETUTCDATE(),

    -- Quantities
    QuantityInspected INT NOT NULL,
    QuantityPassed INT DEFAULT 0,
    QuantityRejected INT DEFAULT 0,

    -- Result
    QCStatus NVARCHAR(20) NOT NULL,
    DefectDescription NVARCHAR(1000),
    RootCause NVARCHAR(500),
    CorrectiveAction NVARCHAR(500),

    -- Disposition
    Disposition NVARCHAR(50),

    -- Photos/Evidence
    PhotoEvidence NVARCHAR(MAX),

    Notes NVARCHAR(1000),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),

    CONSTRAINT FK_QCResults_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Planning_JobCards(Id),

    CONSTRAINT CHK_QCResults_Status CHECK (QCStatus IN ('Pass', 'Fail', 'Conditional Pass', 'Pending'))
);

CREATE INDEX IX_QCResults_JobCard ON Quality_QCResults(JobCardId);
CREATE INDEX IX_QCResults_Status ON Quality_QCResults(QCStatus);
GO

-- =====================================================================
-- DISPATCH SCHEMA
-- =====================================================================

CREATE TABLE Dispatch_DeliveryChallans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ChallanNo NVARCHAR(50) NOT NULL UNIQUE,
    ChallanDate DATETIME DEFAULT GETUTCDATE(),

    -- Order & Customer
    OrderId UNIQUEIDENTIFIER NOT NULL,
    OrderNo NVARCHAR(50),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    CustomerName NVARCHAR(200),
    CustomerAddress NVARCHAR(500),

    -- Delivery Details
    DeliveryAddress NVARCHAR(500),
    DeliveryDate DATETIME,
    VehicleNumber NVARCHAR(50),
    DriverName NVARCHAR(100),
    DriverContact NVARCHAR(20),

    -- Totals
    TotalQuantity INT,
    TotalWeight DECIMAL(10,2),
    TotalValue DECIMAL(18,2),

    -- Status
    Status NVARCHAR(20) DEFAULT 'Prepared',

    -- Transport
    TransporterName NVARCHAR(200),
    LRNumber NVARCHAR(50),
    FreightAmount DECIMAL(18,2),

    -- Approval
    PreparedBy NVARCHAR(100),
    ApprovedBy NVARCHAR(100),
    ApprovedDate DATETIME,

    Notes NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),

    CONSTRAINT FK_Challans_Order FOREIGN KEY (OrderId)
        REFERENCES Orders(Id),
    CONSTRAINT FK_Challans_Customer FOREIGN KEY (CustomerId)
        REFERENCES Masters_Customers(Id),

    CONSTRAINT CHK_Challans_Status CHECK (Status IN ('Prepared', 'Approved', 'In Transit', 'Delivered', 'Cancelled'))
);

CREATE INDEX IX_Challans_ChallanNo ON Dispatch_DeliveryChallans(ChallanNo);
CREATE INDEX IX_Challans_Order ON Dispatch_DeliveryChallans(OrderId);
CREATE INDEX IX_Challans_Customer ON Dispatch_DeliveryChallans(CustomerId);
GO

CREATE TABLE Dispatch_ChallanItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ChallanId UNIQUEIDENTIFIER NOT NULL,
    JobCardId UNIQUEIDENTIFIER,
    JobCardNo NVARCHAR(50),

    ProductName NVARCHAR(200),
    ProductCode NVARCHAR(50),
    Quantity INT NOT NULL,
    UnitOfMeasure NVARCHAR(20),
    Weight DECIMAL(10,2),
    Rate DECIMAL(18,2),
    Amount DECIMAL(18,2),

    SerialNumbers NVARCHAR(MAX),
    Remarks NVARCHAR(500),

    CONSTRAINT FK_ChallanItems_Challan FOREIGN KEY (ChallanId)
        REFERENCES Dispatch_DeliveryChallans(Id),
    CONSTRAINT FK_ChallanItems_JobCard FOREIGN KEY (JobCardId)
        REFERENCES Planning_JobCards(Id),

    CONSTRAINT CHK_ChallanItems_Qty CHECK (Quantity > 0)
);
GO

-- =====================================================================
-- HELPER VIEWS
-- =====================================================================

-- View: Order Summary with Counts
CREATE VIEW vw_OrderSummary AS
SELECT
    o.Id,
    o.OrderNo,
    o.OrderDate,
    o.DueDate,
    c.CustomerName,
    p.ModelName AS ProductName,
    o.Quantity,
    o.QtyCompleted,
    o.QtyInProgress,
    o.QtyRejected,
    o.Status,
    o.PlanningStatus,
    o.DrawingReviewStatus,
    COUNT(DISTINCT jc.Id) AS TotalJobCards,
    COUNT(DISTINCT CASE WHEN jc.Status = 'Completed' THEN jc.Id END) AS CompletedJobCards
FROM Orders o
INNER JOIN Masters_Customers c ON c.Id = o.CustomerId
INNER JOIN Masters_Products p ON p.Id = o.ProductId
LEFT JOIN Planning_JobCards jc ON jc.OrderId = o.Id
GROUP BY o.Id, o.OrderNo, o.OrderDate, o.DueDate, c.CustomerName, p.ModelName,
         o.Quantity, o.QtyCompleted, o.QtyInProgress, o.QtyRejected,
         o.Status, o.PlanningStatus, o.DrawingReviewStatus;
GO

-- View: Material Stock Summary
CREATE VIEW vw_MaterialStockSummary AS
SELECT
    m.Id,
    m.MaterialCode,
    m.MaterialName,
    m.Grade,
    m.UnitOfMeasure,
    COUNT(mp.Id) AS TotalPieces,
    COUNT(CASE WHEN mp.Status = 'Available' THEN mp.Id END) AS AvailablePieces,
    COUNT(CASE WHEN mp.Status = 'Allocated' THEN mp.Id END) AS AllocatedPieces,
    COUNT(CASE WHEN mp.Status = 'Issued' THEN mp.Id END) AS IssuedPieces,
    SUM(CASE WHEN mp.Status = 'Available' THEN mp.CurrentWeightKG ELSE 0 END) AS AvailableStockKG,
    SUM(mp.CurrentWeightKG) AS TotalStockKG
FROM Masters_RawMaterials m
LEFT JOIN Inventory_MaterialPieces mp ON mp.MaterialId = m.Id
GROUP BY m.Id, m.MaterialCode, m.MaterialName, m.Grade, m.UnitOfMeasure;
GO

PRINT 'Phase 1 Schema created successfully!';
GO
