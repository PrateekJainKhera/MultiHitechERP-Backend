namespace MultiHitechERP.API.DTOs.Request
{
    public class ChangeCustomerRequest
    {
        public int CustomerId { get; set; }
        public string? ChangedBy { get; set; }
        public string? Notes { get; set; }
    }
}
