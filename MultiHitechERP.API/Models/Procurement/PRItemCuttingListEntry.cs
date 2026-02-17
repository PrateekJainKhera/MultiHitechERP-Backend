namespace MultiHitechERP.API.Models.Procurement
{
    public class PRItemCuttingListEntry
    {
        public int Id { get; set; }
        public int PRItemId { get; set; }
        public decimal LengthMeter { get; set; }
        public int Pieces { get; set; }
        public string? Notes { get; set; }

        // Computed
        public decimal TotalLengthMeter => LengthMeter * Pieces;
    }
}
