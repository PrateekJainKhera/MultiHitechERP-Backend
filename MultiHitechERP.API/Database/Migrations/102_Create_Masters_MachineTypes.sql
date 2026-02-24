-- Migration 102: Create Masters_MachineTypes table
-- Replaces hardcoded machine type dropdown with a manageable master.

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Masters_MachineTypes')
BEGIN
    CREATE TABLE Masters_MachineTypes (
        Id        INT IDENTITY(1,1) PRIMARY KEY,
        Name      NVARCHAR(100)    NOT NULL,
        IsActive  BIT              NOT NULL DEFAULT 1,
        CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(100)    NULL
    );

    -- Seed with the types that were previously hardcoded in the UI
    INSERT INTO Masters_MachineTypes (Name) VALUES
        ('Lathe'),
        ('CNC Lathe'),
        ('Milling'),
        ('CNC Mill'),
        ('CNC Machining'),
        ('Drilling'),
        ('Grinding'),
        ('Boring'),
        ('Welding'),
        ('Cutting'),
        ('Heat Treatment'),
        ('Finishing'),
        ('Other');

    PRINT 'Created Masters_MachineTypes table with seed data';
END
ELSE
BEGIN
    PRINT 'Masters_MachineTypes table already exists';
END
GO
