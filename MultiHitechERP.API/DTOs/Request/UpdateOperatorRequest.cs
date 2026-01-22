using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateOperatorRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Operator code is required")]
        public string OperatorCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Operator name is required")]
        public string OperatorName { get; set; } = string.Empty;

        // Contact Information
        [EmailAddress(ErrorMessage = "Invalid email address")]
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
        [Range(0, 100, ErrorMessage = "Efficiency rating must be between 0 and 100")]
        public decimal? EfficiencyRating { get; set; }

        [Range(0, 100, ErrorMessage = "Quality rating must be between 0 and 100")]
        public decimal? QualityRating { get; set; }

        // Salary
        [Range(0, double.MaxValue, ErrorMessage = "Hourly rate must be positive")]
        public decimal? HourlyRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Monthly salary must be positive")]
        public decimal? MonthlySalary { get; set; }

        // Status
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public bool IsAvailable { get; set; }

        // Current Assignment (usually set via separate methods)
        public int? CurrentJobCardId { get; set; }
        public string? CurrentJobCardNo { get; set; }
        public int? CurrentMachineId { get; set; }

        public string? Remarks { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
