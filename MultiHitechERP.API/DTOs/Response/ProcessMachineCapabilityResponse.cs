namespace MultiHitechERP.API.DTOs.Response
{
    public class ProcessMachineCapabilityResponse
    {
        public int Id { get; set; }

        // Foreign Keys
        public int ProcessId { get; set; }
        public int MachineId { get; set; }

        // Navigation Properties
        public string? ProcessName { get; set; }
        public string? MachineName { get; set; }
        public string? MachineCode { get; set; }

        // Machine-Specific Time Estimates
        public decimal SetupTimeHours { get; set; }
        public decimal CycleTimePerPieceHours { get; set; }

        // Machine Preference & Capability
        public int PreferenceLevel { get; set; }
        public decimal EfficiencyRating { get; set; }
        public bool IsPreferredMachine { get; set; }

        // Capacity Constraints
        public decimal? MaxWorkpieceLength { get; set; }
        public decimal? MaxWorkpieceDiameter { get; set; }
        public int? MaxBatchSize { get; set; }

        // Cost & Productivity
        public decimal? HourlyRate { get; set; }
        public decimal? EstimatedCostPerPiece { get; set; }

        // Status & Availability
        public bool IsActive { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        // Metadata
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
