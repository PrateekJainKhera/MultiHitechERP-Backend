using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProcessMachineCapabilityRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Process ID is required")]
        public int ProcessId { get; set; }

        [Required(ErrorMessage = "Machine ID is required")]
        public int MachineId { get; set; }

        [Range(0, 999.99, ErrorMessage = "Setup time must be between 0 and 999.99 hours")]
        public decimal SetupTimeHours { get; set; }

        [Range(0, 999.99, ErrorMessage = "Cycle time must be between 0 and 999.99 hours")]
        public decimal CycleTimePerPieceHours { get; set; }

        [Range(1, 5, ErrorMessage = "Preference level must be between 1 (highest) and 5 (lowest)")]
        public int PreferenceLevel { get; set; } = 3;

        [Range(0, 100, ErrorMessage = "Efficiency rating must be between 0 and 100")]
        public decimal EfficiencyRating { get; set; } = 100.00m;

        public bool IsPreferredMachine { get; set; }

        public decimal? MaxWorkpieceLength { get; set; }
        public decimal? MaxWorkpieceDiameter { get; set; }
        public int? MaxBatchSize { get; set; }

        public decimal? HourlyRate { get; set; }
        public decimal? EstimatedCostPerPiece { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        public string? Remarks { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
