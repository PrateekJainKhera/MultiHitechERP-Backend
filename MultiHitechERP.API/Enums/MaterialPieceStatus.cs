namespace MultiHitechERP.API.Enums
{
    /// <summary>
    /// Represents the status of a material piece
    /// </summary>
    public enum MaterialPieceStatus
    {
        Available,
        Allocated,
        Issued,
        Consumed,
        Returned,
        Rejected,
        Scrap
    }
}
