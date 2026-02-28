-- Migration 109: Add MinLengthMM to Masters_Materials
-- MinLengthMM defines the minimum usable length (in mm) for a piece of this material.
-- Pieces shorter than this value are classified as scrap after cutting.
-- Default is 300mm (previously hardcoded across the codebase).

ALTER TABLE Masters_Materials
    ADD MinLengthMM INT NOT NULL DEFAULT 300;
