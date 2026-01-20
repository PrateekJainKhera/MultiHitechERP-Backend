using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class OperatorResponse
    {
        public Guid Id { get; set; }
        public string OperatorCode { get; set; } = string.Empty;
        public string OperatorName { get; set; } = string.Empty;

        // Contact Information
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }

        // Employment Details
        public string? EmployeeId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? ShopFloor { get; set; }

        // Skills & Qualifications
        public string? SkillLevel { get; set; }
        public string? Specialization { get; set; }
        public string? CertificationDetails { get; set; }

        // Expertise (JSON arrays)
        public string? MachineExpertise { get; set; }
        public string? ProcessExpertise { get; set; }

        // Work Schedule
        public string? Shift { get; set; }
        public string? WorkingHours { get; set; }

        // Performance
        public decimal? EfficiencyRating { get; set; }
        public decimal? QualityRating { get; set; }

        // Salary
        public decimal? HourlyRate { get; set; }
        public decimal? MonthlySalary { get; set; }

        // Status
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public bool IsAvailable { get; set; }

        // Current Assignment
        public Guid? CurrentJobCardId { get; set; }
        public string? CurrentJobCardNo { get; set; }
        public Guid? CurrentMachineId { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
