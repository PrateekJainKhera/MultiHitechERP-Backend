-- ============================================================
-- SEED: Bearing & Bush Components
-- Inserts into Masters_Components + Inventory_Stock
-- Run once. Safe to re-run (uses IF NOT EXISTS on PartNumber).
-- ============================================================

SET NOCOUNT ON;

-- ============================================================
-- STEP 1: Insert into Masters_Components catalog
-- ============================================================

DECLARE @inserts TABLE (PartNumber NVARCHAR(100), ComponentName NVARCHAR(200), Manufacturer NVARCHAR(200), Notes NVARCHAR(MAX))

INSERT INTO @inserts VALUES
-- Ball Bearings
('6004-DDUCM-NSK',      '6004 DDUCM Ball Bearing',              'NSK',      '150+300 batch'),
('6005-DDUCM-NSK',      '6005 DDUCM Ball Bearing',              'NSK',      '1000 cut'),
('6204-DDUCM-NSK',      '6204 DDUCM Ball Bearing',              'NSK',      NULL),
('6304-DDUCM-NSK',      '6304 DDUCM Ball Bearing',              'NSK',      NULL),
('6205-DDUCM-NSK',      '6205 DDUCM Ball Bearing',              'NSK',      NULL),
('6206-DDUCM-NSK',      '6206 DDUCM Ball Bearing',              'NSK',      NULL),
('6009-ZZ-C3-NSK',      '6009 ZZ C3 Ball Bearing',              'NSK',      'NS7S written'),
('6009-ZZ-C3-NTN',      '6009 ZZ C3 Ball Bearing',              'NTN',      '04→28 NOS'),
('6917-LLU-NTN',        '6917 LLU Ball Bearing',                'NTN',      '10→06'),
('6007-ZZ-CM-NTN',      '6007 ZZ CM Ball Bearing',              'NTN',      '09→08'),
('6007-DDUCM-NSK',      '6007 DDUCM Ball Bearing',              'NSK',      '02→00'),
('6202-ZZ-SKF',         '6202 ZZ Ball Bearing',                 'SKF',      'NTN→06'),
('6207-ZZ-C3-SKF',      '6207 ZZ C3 Ball Bearing',              'SKF',      NULL),
('6002-DDUCM-NTN',      '6002 DDUCM Ball Bearing',              'NTN',      'NSK NTN'),
('6003-LLU-NTN',        '6003 LLU Ball Bearing',                'NTN',      '06→05'),
('6203-ZZ-C3-NACHI',    '6203 ZZ C3 Ball Bearing',              'NACHI',    'SKF written'),
('6200-ZZ-ECM-NACHI',   '6200 ZZ ECM Ball Bearing',             'NACHI',    NULL),
('R12-ZZ-ITK',          'R12 ZZ Ball Bearing',                  'ITK',      NULL),
('99R12-FBJ',           '99R12 Ball Bearing',                   'FBJ',      '89→84'),
('99R10-FBJ',           '99R10 Ball Bearing',                   'FBJ',      NULL),
('99R8-FBJ',            '99R8 Ball Bearing',                    'FBJ',      '19→17'),
('W5204-2RS-JAF',       'W5204-2RS Angular Contact Bearing',    'JAF',      NULL),
('32008-X-4T-NTN',      '32008 X 4T Taper Roller Bearing',      'NTN',      '06→02'),
('7917-CTYNDUL-P4',     '7917 CTYNDUL Angular Contact Bearing', 'P4',       NULL),
('7917-CTYNSUL-P4',     '7917 CTYNSUL Angular Contact Bearing', 'P4',       NULL),
('7917-CTRDUL-RHP',     '7917 CTRDUL Angular Contact Bearing',  'RHP',      NULL),
('RCB-162117',          'RCB 162117 Needle Bearing',            NULL,       NULL),
('LRB-1216',            'LRB 1216 Bearing',                     NULL,       NULL),
-- Needle Bearings
('RNA4905-XL-VIETNAM',  'RNA4905-XL Needle Bearing',            'VIETNAM',  'AM 0358 Z126'),
('RNA4904R-NTN',        'RNA4904R Needle Roll Bearing',         'NTN',      NULL),
('IR25x30x30',          'IR25x30x30 Needle Roller Bearing',     NULL,       NULL),
('INA-KRV30-PP-A',      'INA KRV30-PP-A Cam Follower',         'INA',      NULL),
('INA-KRV22-B',         'INA KRV22-B Cam Follower',            'INA',      NULL),
('INA-KRV26-PP-A',      'INA KRV26-PP-A Cam Follower',         'INA',      NULL),
('INA-KRV32-PP-A',      'INA KRV32-PP-A Cam Follower',         'INA',      NULL),
('INA-KR32-PP-A',       'INA KR32-PP-A Cam Follower',          'INA',      NULL),
-- DU Bushes
('DU-BUSH-22x15x25',    'DU Bush 22x15x25',                    NULL,       NULL),
('DU-BUSH-28x25x32',    'DU Bush 28x25x32',                    NULL,       NULL),
('DU-BUSH-65x16x70',    'DU Bush 65x16x70',                    NULL,       NULL),
('DU-BUSH-20x15x23',    'DU Bush 20x15x23',                    NULL,       NULL),
('DU-BUSH-PAP-15x25-28','DU Bush PAP 15x25-28 OGA P10',        NULL,       'OGA P10'),
-- Inner Races
('IR-30x25x17',         'Inner Race 30x25x17',                 NULL,       NULL),
('IR-25x20x17',         'Inner Race 25x20x17',                 NULL,       NULL),
('IR-35x40x40',         'IR35x40x40 Inner Race',               NULL,       NULL),
('IR-40x48x22',         'Inner Race 40x48x22',                 NULL,       NULL),
('IR-40x40x40',         'Inner Race 40x40x40',                 NULL,       NULL),
('IR-38x32x32',         'Inner Race 38x32x32',                 NULL,       NULL),
('IR-35x30x30',         'Inner Race 35x30x30',                 NULL,       NULL),
('IR-30x28x22',         'Inner Race 30x28x22',                 NULL,       NULL),
('IR-25x25x19',         'Inner Race 25x25x19',                 NULL,       NULL),
('IR-28x22x17',         'Inner Race 28x22x17',                 NULL,       NULL),
('IR-20x18x15',         'Inner Race 20x18x15',                 NULL,       NULL),
('IR-23x20x15',         'Inner Race 23x20x15',                 NULL,       NULL),
-- Misc Bearings
('6905-LLU-2AS',        '6905 LLU / 2AS Bearing',              NULL,       NULL),
('6010-DDUCM-NSK',      '6010 DDUCM Ball Bearing',             'NSK',      NULL),
('6001-2N-ECM-NACHI',   '6001-2N ECM Ball Bearing',            'NACHI',    NULL),
('6917-LLU-2AS-NTN',    '6917 LLU / 2AS Ball Bearing',         'NTN',      'OLD');

-- Insert only if PartNumber doesn't already exist
INSERT INTO Masters_Components (PartNumber, ComponentName, Category, Manufacturer, Unit, Notes, LeadTimeDays, IsActive, CreatedBy)
SELECT
    i.PartNumber,
    i.ComponentName,
    CASE
        WHEN i.PartNumber LIKE 'DU-BUSH%'   THEN 'Bush'
        WHEN i.PartNumber LIKE 'IR-%'       THEN 'Inner Race'
        WHEN i.PartNumber LIKE 'INA-%'      THEN 'Cam Follower'
        WHEN i.PartNumber LIKE 'RNA%'
          OR i.PartNumber LIKE 'IR25%'
          OR i.PartNumber LIKE 'RCB%'
          OR i.PartNumber LIKE 'LRB%'       THEN 'Needle Bearing'
        ELSE 'Bearing'
    END AS Category,
    i.Manufacturer,
    'PCS',
    i.Notes,
    0,      -- LeadTimeDays
    1,      -- IsActive
    'Admin'
FROM @inserts i
WHERE NOT EXISTS (
    SELECT 1 FROM Masters_Components mc WHERE mc.PartNumber = i.PartNumber
);

PRINT CAST(@@ROWCOUNT AS NVARCHAR) + ' components inserted into Masters_Components.';
GO

-- ============================================================
-- STEP 2: Insert current stock into Inventory_Stock
-- ============================================================

DECLARE @stock TABLE (PartNumber NVARCHAR(100), Qty DECIMAL(18,3))

INSERT INTO @stock VALUES
('6004-DDUCM-NSK',       450),
('6005-DDUCM-NSK',       900),
('6204-DDUCM-NSK',        32),
('6304-DDUCM-NSK',       100),
('6205-DDUCM-NSK',       135),
('6206-DDUCM-NSK',       100),
('6009-ZZ-C3-NSK',         1),
('6009-ZZ-C3-NTN',        28),
('6917-LLU-NTN',           6),
('6007-ZZ-CM-NTN',         8),
('6007-DDUCM-NSK',         0),
('6202-ZZ-SKF',            6),
('6207-ZZ-C3-SKF',         2),
('6002-DDUCM-NTN',        10),
('6003-LLU-NTN',           5),
('6203-ZZ-C3-NACHI',       1),
('6200-ZZ-ECM-NACHI',      1),
('R12-ZZ-ITK',             4),
('99R12-FBJ',             84),
('99R10-FBJ',             20),
('99R8-FBJ',              17),
('W5204-2RS-JAF',         42),
('32008-X-4T-NTN',         2),
('7917-CTYNDUL-P4',        2),
('7917-CTYNSUL-P4',        2),
('7917-CTRDUL-RHP',        4),
('RCB-162117',             4),
('LRB-1216',               4),
('RNA4905-XL-VIETNAM',    10),
('RNA4904R-NTN',          15),
('IR25x30x30',             6),
('INA-KRV30-PP-A',         4),
('INA-KRV22-B',            3),
('INA-KRV26-PP-A',         3),
('INA-KRV32-PP-A',         2),
('INA-KR32-PP-A',          3),
('DU-BUSH-22x15x25',      44),
('DU-BUSH-28x25x32',      51),
('DU-BUSH-65x16x70',      10),
('DU-BUSH-20x15x23',      65),
('DU-BUSH-PAP-15x25-28',  67),
('IR-30x25x17',           26),
('IR-25x20x17',            1),
('IR-35x40x40',            4),
('IR-40x48x22',            6),
('IR-40x40x40',            5),
('IR-38x32x32',           10),
('IR-35x30x30',            3),
('IR-30x28x22',            5),
('IR-25x25x19',           12),
('IR-28x22x17',            3),
('IR-20x18x15',            1),
('IR-23x20x15',            1),
('6905-LLU-2AS',         119),
('6010-DDUCM-NSK',         1),
('6001-2N-ECM-NACHI',     14),
('6917-LLU-2AS-NTN',       1);

-- Insert stock only for components that exist in Masters_Components and not yet in Inventory_Stock
INSERT INTO Inventory_Stock (ItemType, ItemId, ItemCode, ItemName, CurrentStock, ReservedStock, UOM, UpdatedBy)
SELECT
    'Component',
    mc.Id,
    mc.PartNumber,
    mc.ComponentName,
    s.Qty,
    0,
    'PCS',
    'Admin'
FROM @stock s
JOIN Masters_Components mc ON mc.PartNumber = s.PartNumber
WHERE s.Qty > 0
  AND NOT EXISTS (
    SELECT 1 FROM Inventory_Stock is2
    WHERE is2.ItemType = 'Component' AND is2.ItemId = mc.Id
  );

PRINT CAST(@@ROWCOUNT AS NVARCHAR) + ' stock records inserted into Inventory_Stock.';
GO

PRINT 'Bearing seed complete.';
