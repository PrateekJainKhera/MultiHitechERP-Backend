-- Migration 071: Create Roller Types Master Table
-- Replaces hardcoded RollerType enum (Magnetic Roller, Printing Roller) with DB-driven master

CREATE TABLE Masters_RollerTypes (
    Id          INT PRIMARY KEY IDENTITY(1,1),
    TypeName    NVARCHAR(100) NOT NULL,
    IsActive    BIT NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100) NULL
);

-- Seed with existing types (must match values stored in Masters_Products.RollerType)
INSERT INTO Masters_RollerTypes (TypeName, CreatedBy) VALUES
('Magnetic Roller', 'System'),
('Printing Roller', 'System');
