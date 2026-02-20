-- Migration 091: Create Shift Master table for scheduling
-- Stores shift definitions used for machine scheduling

CREATE TABLE Scheduling_ShiftMaster (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShiftName NVARCHAR(100) NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    RegularHours DECIMAL(5,2) NOT NULL DEFAULT 8.0,
    MaxOvertimeHours DECIMAL(5,2) NOT NULL DEFAULT 3.0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(200) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(200) NULL
);
GO

-- Insert default shifts
INSERT INTO Scheduling_ShiftMaster (ShiftName, StartTime, EndTime, RegularHours, MaxOvertimeHours, IsActive)
VALUES
    ('Shift A', '08:00:00', '16:00:00', 8.0, 3.0, 1),
    ('Shift B', '16:00:00', '00:00:00', 8.0, 3.0, 1),
    ('Night Shift', '00:00:00', '08:00:00', 8.0, 3.0, 1);
GO
