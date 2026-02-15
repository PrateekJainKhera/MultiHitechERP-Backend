-- Migration 063: Add ProductTemplateId to Products table
-- This stores which specific Product Template was used when creating a product
-- Previously only ProcessTemplateId was stored, making it hard to retrieve the exact template with child parts

USE MultiHitechERP;
GO

-- Add ProductTemplateId column
ALTER TABLE Masters_Products
ADD ProductTemplateId INT NULL;
GO

-- Add foreign key constraint
ALTER TABLE Masters_Products
ADD CONSTRAINT FK_Products_ProductTemplate
FOREIGN KEY (ProductTemplateId) REFERENCES Masters_ProductTemplates(Id);
GO

-- Create index for performance
SET QUOTED_IDENTIFIER ON;
GO
CREATE INDEX IX_Products_ProductTemplateId
ON Masters_Products(ProductTemplateId);
GO

-- Backfill ProductTemplateId based on ProcessTemplateId (get first matching template)
-- This is a best-effort migration for existing data
SET QUOTED_IDENTIFIER ON;
GO
UPDATE p
SET p.ProductTemplateId = (
    SELECT TOP 1 pt.Id
    FROM Masters_ProductTemplates pt
    WHERE pt.ProcessTemplateId = p.ProcessTemplateId
      AND pt.RollerType = p.RollerType
      AND pt.IsActive = 1
    ORDER BY pt.CreatedAt ASC
)
FROM Masters_Products p
WHERE p.ProductTemplateId IS NULL;
GO

PRINT 'Migration 063 completed: ProductTemplateId added to Products table';
GO
