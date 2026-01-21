using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for material piece data
    /// </summary>
    public class MaterialPieceResponse
    {
        public Guid Id { get; set; }
        public Guid MaterialId { get; set; }
        public string PieceNo { get; set; } = string.Empty;

        public decimal OriginalLengthMM { get; set; }
        public decimal CurrentLengthMM { get; set; }
        public decimal OriginalWeightKG { get; set; }
        public decimal CurrentWeightKG { get; set; }

        public string Status { get; set; } = string.Empty;
        public Guid? AllocatedToRequisitionId { get; set; }
        public Guid? IssuedToJobCardId { get; set; }

        public string? StorageLocation { get; set; }
        public string? BinNumber { get; set; }
        public string? RackNumber { get; set; }

        public string? GRNNo { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? SupplierBatchNo { get; set; }
        public Guid? SupplierId { get; set; }
        public decimal? UnitCost { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
