-- =============================================
-- Migration: Create Scheduling_MachineSchedules table
-- Purpose: Track time slots allocated to machines for job cards
-- Date: 2026-02-07
-- =============================================

-- Scheduling_MachineSchedules: Time slots booked on machines for job cards
CREATE TABLE Scheduling_MachineSchedules (
    Id INT IDENTITY(1,1) PRIMARY KEY,

    -- Job Card linkage
    JobCardId INT NOT NULL,
    JobCardNo NVARCHAR(50) NULL,

    -- Machine assignment
    MachineId INT NOT NULL,
    MachineCode NVARCHAR(50) NULL,
    MachineName NVARCHAR(200) NULL,

    -- Scheduling information
    ScheduledStartTime DATETIME2 NOT NULL,
    ScheduledEndTime DATETIME2 NOT NULL,
    EstimatedDurationMinutes INT NOT NULL,

    -- Actual time tracking (populated during production)
    ActualStartTime DATETIME2 NULL,
    ActualEndTime DATETIME2 NULL,
    ActualDurationMinutes INT NULL,

    -- Status
    Status NVARCHAR(50) NOT NULL DEFAULT 'Scheduled',  -- Scheduled | InProgress | Completed | Cancelled | Rescheduled

    -- Scheduling metadata
    SchedulingMethod NVARCHAR(50) NOT NULL DEFAULT 'Semi-Automatic',  -- Semi-Automatic | Manual | Auto
    SuggestedBySystem BIT NOT NULL DEFAULT 0,
    ConfirmedBy NVARCHAR(200) NULL,
    ConfirmedAt DATETIME2 NULL,

    -- Process info (denormalized for quick access)
    ProcessId INT NOT NULL,
    ProcessName NVARCHAR(200) NULL,
    ProcessCode NVARCHAR(50) NULL,

    -- Rescheduling tracking
    IsRescheduled BIT NOT NULL DEFAULT 0,
    RescheduledFromId INT NULL,  -- Points to original schedule if rescheduled
    RescheduledReason NVARCHAR(500) NULL,
    RescheduledAt DATETIME2 NULL,
    RescheduledBy NVARCHAR(200) NULL,

    -- Notes
    Notes NVARCHAR(MAX) NULL,

    -- Audit
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(200) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(200) NULL,

    -- Foreign Keys
    CONSTRAINT FK_Schedule_JobCard FOREIGN KEY (JobCardId) REFERENCES Planning_JobCards(Id),
    CONSTRAINT FK_Schedule_Machine FOREIGN KEY (MachineId) REFERENCES Masters_Machines(Id),
    CONSTRAINT FK_Schedule_Process FOREIGN KEY (ProcessId) REFERENCES Masters_Processes(Id),
    CONSTRAINT FK_Schedule_RescheduledFrom FOREIGN KEY (RescheduledFromId) REFERENCES Scheduling_MachineSchedules(Id)
);
GO

-- Indexes for performance
CREATE INDEX IX_Schedule_JobCardId ON Scheduling_MachineSchedules (JobCardId);
CREATE INDEX IX_Schedule_MachineId ON Scheduling_MachineSchedules (MachineId);
CREATE INDEX IX_Schedule_Status ON Scheduling_MachineSchedules (Status);
CREATE INDEX IX_Schedule_StartTime ON Scheduling_MachineSchedules (ScheduledStartTime);
CREATE INDEX IX_Schedule_EndTime ON Scheduling_MachineSchedules (ScheduledEndTime);
CREATE INDEX IX_Schedule_MachineStartTime ON Scheduling_MachineSchedules (MachineId, ScheduledStartTime, ScheduledEndTime);
GO

-- Trigger to prevent overlapping schedules on same machine
CREATE TRIGGER TR_PreventOverlappingSchedules
ON Scheduling_MachineSchedules
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for overlapping schedules
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Scheduling_MachineSchedules existing
            ON i.MachineId = existing.MachineId
            AND i.Id != existing.Id
            AND existing.Status IN ('Scheduled', 'InProgress')
        WHERE (
            -- New schedule starts during existing schedule
            (i.ScheduledStartTime >= existing.ScheduledStartTime AND i.ScheduledStartTime < existing.ScheduledEndTime)
            OR
            -- New schedule ends during existing schedule
            (i.ScheduledEndTime > existing.ScheduledStartTime AND i.ScheduledEndTime <= existing.ScheduledEndTime)
            OR
            -- New schedule completely encompasses existing schedule
            (i.ScheduledStartTime <= existing.ScheduledStartTime AND i.ScheduledEndTime >= existing.ScheduledEndTime)
        )
    )
    BEGIN
        RAISERROR('Schedule conflict detected: Machine already has overlapping schedule for this time period.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

PRINT 'Scheduling_MachineSchedules table created successfully';
