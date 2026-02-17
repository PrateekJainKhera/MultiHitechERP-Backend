using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class MaterialPieceResponse
    {
        public int Id { get; set; }
        public string PieceNo { get; set; } = string.Empty;
        public int MaterialId { get; set; }

        // Denormalized Material Info
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? Grade { get; set; }
        public decimal? Diameter { get; set; }

        // Length & Weight (stored in MM and KG, can be converted to M in frontend)
        public decimal OriginalLengthMM { get; set; }
        public decimal CurrentLengthMM { get; set; }
        public decimal OriginalWeightKG { get; set; }
        public decimal CurrentWeightKG { get; set; }

        // Calculated fields for UI convenience
        public decimal CurrentLengthMeters => CurrentLengthMM / 1000m;
        public decimal OriginalLengthMeters => OriginalLengthMM / 1000m;
        public decimal UsagePercentage => OriginalLengthMM > 0
            ? Math.Round(((OriginalLengthMM - CurrentLengthMM) / OriginalLengthMM) * 100, 2)
            : 0;

        // Status
        public string Status { get; set; } = string.Empty;
        public int? AllocatedToRequisitionId { get; set; }
        public int? IssuedToJobCardId { get; set; }

        // Location
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string? StorageLocation { get; set; }
        public string? BinNumber { get; set; }
        public string? RackNumber { get; set; }

        // GRN Reference
        public int? GRNId { get; set; }
        public string? GRNNo { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? SupplierBatchNo { get; set; }
        public int? SupplierId { get; set; }
        public decimal? UnitCost { get; set; }

        // Wastage
        public bool IsWastage { get; set; }
        public string? WastageReason { get; set; }
        public decimal? ScrapValue { get; set; }

        // Calculated current value
        public decimal? CurrentValue => OriginalLengthMM > 0 && UnitCost.HasValue
            ? Math.Round((UnitCost.Value / OriginalLengthMM) * CurrentLengthMM, 2)
            : null;

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
