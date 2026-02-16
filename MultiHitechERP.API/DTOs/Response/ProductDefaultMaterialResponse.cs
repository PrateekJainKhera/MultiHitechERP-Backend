namespace MultiHitechERP.API.DTOs.Response
{
    public class ProductDefaultMaterialResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ChildPartTemplateId { get; set; }
        public int? RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
        public string MaterialGrade { get; set; } = string.Empty;
        public decimal RequiredQuantity { get; set; }
        public string Unit { get; set; } = "mm";
        public decimal WastageMM { get; set; }
        public string? Notes { get; set; }
    }
}
