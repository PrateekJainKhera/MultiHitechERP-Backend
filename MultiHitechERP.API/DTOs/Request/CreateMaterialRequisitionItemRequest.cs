using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a material requisition item (line item)
    /// </summary>
    public class CreateMaterialRequisitionItemRequest
    {
        [Required(ErrorMessage = "Line number is required")]
        public int LineNo { get; set; }

        // Raw material (set when item is for a raw material rod/bar; mutually exclusive with ComponentId)
        public int? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialGrade { get; set; }

        // Purchased component (set when item is for a received component; mutually exclusive with MaterialId)
        public int? ComponentId { get; set; }
        public string? ComponentCode { get; set; }
        public string? ComponentName { get; set; }

        [Required(ErrorMessage = "Quantity required is mandatory")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal QuantityRequired { get; set; }

        public string? UOM { get; set; } = "KG";

        // Length-based (for rods/pipes)
        public decimal? LengthRequiredMM { get; set; }
        public decimal? DiameterMM { get; set; }
        public int? NumberOfPieces { get; set; }

        // Reference
        public int? JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public int? ProcessId { get; set; }
        public string? ProcessName { get; set; }

        // Pre-allocated pieces (optional - for piece-level inventory control)
        /// <summary>
        /// Optional list of pre-selected material piece IDs for this requisition item.
        /// If provided, these pieces will be allocated instead of using automatic FIFO allocation.
        /// </summary>
        public List<int>? SelectedPieceIds { get; set; }

        /// <summary>
        /// Optional list of cut quantities (in MM) for each selected piece.
        /// Order matches SelectedPieceIds. E.g., [300, 500, 200] means cut 300mm from piece 1, 500mm from piece 2, etc.
        /// </summary>
        public List<decimal>? SelectedPieceQuantities { get; set; }

        public string? Remarks { get; set; }
    }
}
