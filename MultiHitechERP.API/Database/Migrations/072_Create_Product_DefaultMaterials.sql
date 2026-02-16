-- Migration 072: Create Product Default Materials Table
-- Stores default material requirements per product so planners don't have to re-enter
-- them every time a new order comes in for the same product.
-- ChildPartTemplateId = NULL means the main assembly material

CREATE TABLE Product_DefaultMaterials (
    Id                  INT PRIMARY KEY IDENTITY(1,1),
    ProductId           INT NOT NULL,
    ChildPartTemplateId INT NULL,           -- NULL = main assembly; FK = specific child part
    RawMaterialId       INT NULL,           -- FK to Masters_Materials (optional)
    RawMaterialName     NVARCHAR(200) NOT NULL,
    MaterialGrade       NVARCHAR(100) NOT NULL DEFAULT '',
    RequiredQuantity    DECIMAL(18,4)  NOT NULL DEFAULT 0,
    Unit                NVARCHAR(20)   NOT NULL DEFAULT 'mm',
    WastageMM           DECIMAL(5,2)   NOT NULL DEFAULT 5,
    Notes               NVARCHAR(500)  NULL,
    CreatedAt           DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           NVARCHAR(100)  NULL,
    UpdatedAt           DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy           NVARCHAR(100)  NULL,

    CONSTRAINT FK_ProductDefaultMaterials_Product
        FOREIGN KEY (ProductId) REFERENCES Masters_Products(Id) ON DELETE CASCADE
);

CREATE INDEX IX_ProductDefaultMaterials_ProductId
    ON Product_DefaultMaterials(ProductId);
