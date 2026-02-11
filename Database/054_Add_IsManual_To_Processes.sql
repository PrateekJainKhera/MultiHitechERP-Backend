-- Migration 054: Add IsManual flag to Masters_Processes
-- Manual processes (e.g. hand fitting, inspection, manual assembly) do not require
-- a machine to be assigned in scheduling — they behave like OSP but are done in-house.

ALTER TABLE Masters_Processes
    ADD IsManual BIT NOT NULL DEFAULT 0;
GO

-- Seed known manual processes
-- Adjust names to match exactly what is in your Masters_Processes table
UPDATE Masters_Processes SET IsManual = 1 WHERE
    ProcessName LIKE '%Quality Inspection%'
    OR ProcessName LIKE '%QA Inspection%'
    OR ProcessName LIKE '%Bearers Fitting%'
    OR ProcessName LIKE '%Magnet Assembly%'
    OR ProcessName LIKE '%Chemical Assembly%'
    OR ProcessName LIKE '%(manual)%';
GO

-- Show what was marked
SELECT Id, ProcessCode, ProcessName, IsManual
FROM Masters_Processes
WHERE IsManual = 1;
GO
