-- Migration 108: Create Masters_MaterialTypes table
-- Replaces hardcoded MaterialType enum with admin-managed master

CREATE TABLE Masters_MaterialTypes (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Name      NVARCHAR(100) NOT NULL,
    IsActive  BIT           NOT NULL DEFAULT 1,
    CreatedAt DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL
);

-- Seed with common material types
INSERT INTO Masters_MaterialTypes (Name) VALUES
    ('Steel (MS / EN8)'),
    ('Alloy Steel (EN19)'),
    ('Alloy Steel (EN24)'),
    ('Stainless Steel'),
    ('Aluminum'),
    ('Brass'),
    ('Cast Iron'),
    ('Copper'),
    ('Other');

GO

PRINT 'Migration 108: Masters_MaterialTypes created and seeded';
