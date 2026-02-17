using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateEstimationRequest
    {
        [Required]
        public int CustomerId { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public string? Notes { get; set; }
        public string? TermsAndConditions { get; set; }
        [Required]
        public List<CreateEstimationItemRequest> Items { get; set; } = new();
    }

    public class CreateEstimationItemRequest
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
    }
}
