-- Migration 070: Create Child Part Types Master Table
-- Replaces hardcoded CHILD_PART_TYPES array in frontend with DB-driven master

CREATE TABLE Masters_ChildPartTypes (
    Id          INT PRIMARY KEY IDENTITY(1,1),
    TypeName    NVARCHAR(50) NOT NULL,
    IsActive    BIT NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100) NULL
);

-- Seed with existing hardcoded types
INSERT INTO Masters_ChildPartTypes (TypeName, CreatedBy) VALUES
('SHAFT',   'System'),
('SLEEVE',  'System'),
('TIKKI',   'System'),
('GEAR',    'System'),
('ENDS',    'System'),
('PIPE',    'System'),
('BEARING', 'System'),
('PATTI',   'System'),
('MAGNET',  'System'),
('OTHER',   'System');
