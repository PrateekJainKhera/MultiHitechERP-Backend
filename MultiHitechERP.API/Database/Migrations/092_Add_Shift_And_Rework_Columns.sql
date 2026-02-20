-- Migration 092: Add ShiftId to MachineSchedules + rework columns to JobCards

-- Add shift tracking to machine schedules
ALTER TABLE Scheduling_MachineSchedules
ADD ShiftId INT NULL,
    ShiftName NVARCHAR(100) NULL;
GO

ALTER TABLE Scheduling_MachineSchedules
ADD CONSTRAINT FK_MachineSchedules_Shift
    FOREIGN KEY (ShiftId) REFERENCES Scheduling_ShiftMaster(Id);
GO

-- Add rework columns to job cards
ALTER TABLE Planning_JobCards
ADD IsRework BIT NOT NULL DEFAULT 0,
    ParentJobCardId INT NULL,
    JobCardType NVARCHAR(50) NOT NULL DEFAULT 'Normal';
GO

ALTER TABLE Planning_JobCards
ADD CONSTRAINT FK_JobCards_ParentJobCard
    FOREIGN KEY (ParentJobCardId) REFERENCES Planning_JobCards(Id);
GO
