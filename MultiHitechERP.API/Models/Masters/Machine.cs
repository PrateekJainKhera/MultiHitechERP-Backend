using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a machine master record
    /// </summary>
    public class Machine
    {
        public Guid Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;

        // Classification
        public string? MachineType { get; set; }
        public string? Category { get; set; }

        // Technical Details
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public int? YearOfManufacture { get; set; }

        // Capacity & Specifications
        public decimal? Capacity { get; set; }
        public string? CapacityUnit { get; set; }
        public string? Specifications { get; set; }

        // Dimensions
        public decimal? MaxWorkpieceLength { get; set; }
        public decimal? MaxWorkpieceDiameter { get; set; }
        public decimal? ChuckSize { get; set; }

        // Location
        public string? Department { get; set; }
        public string? ShopFloor { get; set; }
        public string? Location { get; set; }

        // Operational Details
        public decimal? HourlyRate { get; set; }
        public decimal? PowerConsumption { get; set; }
        public int? OperatorsRequired { get; set; }

        // Maintenance
        public DateTime? PurchaseDate { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string? MaintenanceSchedule { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public string? Status { get; set; } = "Available";
        public string? CurrentJobCardNo { get; set; }

        // Availability
        public bool IsAvailable { get; set; } = true;
        public DateTime? AvailableFrom { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
