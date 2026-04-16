-- Migration 114: Fix "Magnet Roller" → "Magnetic Roller" across all tables
-- Also updates Masters_ProcessTemplates.ApplicableTypes JSON
-- Safe to re-run (uses conditional updates)

PRINT 'Migration 114: Fixing Magnetic Roller name across all tables...';
GO

-- ============================================================
-- 1. Masters_RollerTypes (dropdown source)
-- ============================================================
IF EXISTS (SELECT 1 FROM Masters_RollerTypes WHERE TypeName = 'Magnet Roller')
BEGIN
    UPDATE Masters_RollerTypes SET TypeName = 'Magnetic Roller' WHERE TypeName = 'Magnet Roller';
    PRINT '  Fixed Masters_RollerTypes: Magnet Roller → Magnetic Roller';
END
ELSE
BEGIN
    PRINT '  Masters_RollerTypes: already correct (or already fixed)';
END
GO

-- ============================================================
-- 2. Masters_ProductTemplates.RollerType
-- ============================================================
IF EXISTS (SELECT 1 FROM Masters_ProductTemplates WHERE RollerType = 'Magnet Roller')
BEGIN
    UPDATE Masters_ProductTemplates SET RollerType = 'Magnetic Roller' WHERE RollerType = 'Magnet Roller';
    PRINT '  Fixed Masters_ProductTemplates.RollerType';
END
ELSE
BEGIN
    PRINT '  Masters_ProductTemplates: no Magnet Roller entries found';
END
GO

-- ============================================================
-- 3. Masters_Products.RollerType (existing products)
-- ============================================================
IF EXISTS (SELECT 1 FROM Masters_Products WHERE RollerType = 'Magnet Roller')
BEGIN
    UPDATE Masters_Products SET RollerType = 'Magnetic Roller' WHERE RollerType = 'Magnet Roller';
    PRINT '  Fixed Masters_Products.RollerType';
END
ELSE
BEGIN
    PRINT '  Masters_Products: no Magnet Roller entries found';
END
GO

-- ============================================================
-- 4. Masters_ProcessTemplates.ApplicableTypes (JSON array)
--    Handles both ["MAGNET"] and ["Magnet Roller"] formats
-- ============================================================
IF EXISTS (SELECT 1 FROM Masters_ProcessTemplates WHERE ApplicableTypes LIKE '%Magnet Roller%')
BEGIN
    UPDATE Masters_ProcessTemplates
    SET ApplicableTypes = REPLACE(ApplicableTypes, 'Magnet Roller', 'Magnetic Roller')
    WHERE ApplicableTypes LIKE '%Magnet Roller%';
    PRINT '  Fixed Masters_ProcessTemplates.ApplicableTypes (Magnet Roller → Magnetic Roller)';
END
ELSE
BEGIN
    PRINT '  Masters_ProcessTemplates (Magnet Roller): no entries to fix';
END

IF EXISTS (SELECT 1 FROM Masters_ProcessTemplates WHERE ApplicableTypes LIKE '%"MAGNET"%')
BEGIN
    UPDATE Masters_ProcessTemplates
    SET ApplicableTypes = REPLACE(ApplicableTypes, '"MAGNET"', '"MAGNETIC"')
    WHERE ApplicableTypes LIKE '%"MAGNET"%';
    PRINT '  Fixed Masters_ProcessTemplates.ApplicableTypes (MAGNET → MAGNETIC)';
END
ELSE
BEGIN
    PRINT '  Masters_ProcessTemplates (MAGNET): no entries to fix';
END
GO

-- ============================================================
-- 5. Show current state for verification
-- ============================================================
PRINT '';
PRINT '=== Verification: Current roller type data ===';

PRINT '--- Masters_RollerTypes ---';
SELECT Id, TypeName FROM Masters_RollerTypes ORDER BY TypeName;

PRINT '--- Masters_ProductTemplates ---';
SELECT Id, TemplateName, RollerType FROM Masters_ProductTemplates ORDER BY RollerType, TemplateName;

PRINT '--- Masters_ProcessTemplates (ApplicableTypes) ---';
SELECT Id, TemplateName, ApplicableTypes FROM Masters_ProcessTemplates ORDER BY TemplateName;
GO

PRINT 'Migration 114 complete.';
GO
