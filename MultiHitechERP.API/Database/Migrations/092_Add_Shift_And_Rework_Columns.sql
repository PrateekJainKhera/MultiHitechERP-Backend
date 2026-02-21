-- Migration 092: Add ShiftId to MachineSchedules + rework columns to JobCards

-- Add shift tracking to machine schedules
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Scheduling_MachineSchedules') AND name = 'ShiftId')
    ALTER TABLE Scheduling_MachineSchedules ADD ShiftId INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Scheduling_MachineSchedules') AND name = 'ShiftName')
    ALTER TABLE Scheduling_MachineSchedules ADD ShiftName NVARCHAR(100) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MachineSchedules_Shift')
    ALTER TABLE Scheduling_MachineSchedules
    ADD CONSTRAINT FK_MachineSchedules_Shift
        FOREIGN KEY (ShiftId) REFERENCES Scheduling_ShiftMaster(Id);
GO

-- Add rework columns to job cards
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'IsRework')
    ALTER TABLE Planning_JobCards ADD IsRework BIT NOT NULL DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'ParentJobCardId')
    ALTER TABLE Planning_JobCards ADD ParentJobCardId INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Planning_JobCards') AND name = 'JobCardType')
    ALTER TABLE Planning_JobCards ADD JobCardType NVARCHAR(50) NOT NULL DEFAULT 'Normal';
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_JobCards_ParentJobCard')
    ALTER TABLE Planning_JobCards
    ADD CONSTRAINT FK_JobCards_ParentJobCard
        FOREIGN KEY (ParentJobCardId) REFERENCES Planning_JobCards(Id);
GO
