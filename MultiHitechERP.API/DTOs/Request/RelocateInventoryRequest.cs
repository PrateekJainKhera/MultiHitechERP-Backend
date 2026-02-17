using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class RelocateInventoryRequest
    {
        [Required]
        public List<int> PieceIds { get; set; } = new();  // Single or bulk: one or many piece IDs

        [Required]
        public int ToWarehouseId { get; set; }            // Destination warehouse

        public string? RelocatedBy { get; set; }
    }
}
