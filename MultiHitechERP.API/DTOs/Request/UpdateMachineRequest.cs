using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateMachineRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Machine code is required")]
        public string MachineCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Machine name is required")]
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
        public bool IsActive { get; set; }
        public string? Status { get; set; }

        public string? Remarks { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
