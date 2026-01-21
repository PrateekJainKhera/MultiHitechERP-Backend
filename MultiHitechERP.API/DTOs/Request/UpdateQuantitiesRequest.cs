using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating production quantities during execution
    /// </summary>
    public class UpdateQuantitiesRequest
    {
        [Range(0, int.MaxValue, ErrorMessage = "Quantity completed cannot be negative")]
        public int? QuantityCompleted { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity rejected cannot be negative")]
        public int? QuantityRejected { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity in progress cannot be negative")]
        public int? QuantityInProgress { get; set; }
    }
}
