-- Migration 105: Add ReceivedQty + RejectedQty to Production_OSPTracking
-- Enables partial receive tracking (e.g., 5 of 10 gears received)

ALTER TABLE Production_OSPTracking ADD ReceivedQty INT NOT NULL DEFAULT 0;
ALTER TABLE Production_OSPTracking ADD RejectedQty INT NOT NULL DEFAULT 0;
