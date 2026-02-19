-- Migration 090: Add Finalized status to Issue Window Drafts
-- Separates Finalize (reserve/lock) from Issue (physical cut/irreversible)

-- 1. Drop old CHECK constraint (only allowed Draft / Issued)
ALTER TABLE Stores_IssueWindowDrafts
DROP CONSTRAINT CHK_IssueWindowDraft_Status;

-- 2. Add FinalizedAt column
ALTER TABLE Stores_IssueWindowDrafts
ADD FinalizedAt DATETIME NULL;

-- 3. Add new CHECK constraint that includes Finalized
ALTER TABLE Stores_IssueWindowDrafts
ADD CONSTRAINT CHK_IssueWindowDraft_Status
CHECK (Status IN ('Draft', 'Finalized', 'Issued'));
