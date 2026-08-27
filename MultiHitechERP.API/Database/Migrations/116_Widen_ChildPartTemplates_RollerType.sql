-- Migration 116: Widen Masters_ChildPartTemplates.RollerType
-- The child-part-template form is a multi-select and stores ALL chosen roller
-- types comma-joined into this single column. NVARCHAR(50) is too small — picking
-- enough roller types (e.g. adding 'Chill Roller') overflows 50 chars and the save
-- fails with "String or binary data would be truncated". Widen to hold the list.

ALTER TABLE Masters_ChildPartTemplates ALTER COLUMN RollerType NVARCHAR(500) NULL;

PRINT 'Masters_ChildPartTemplates.RollerType widened to NVARCHAR(500).';
