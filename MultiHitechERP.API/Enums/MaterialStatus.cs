namespace MultiHitechERP.API.Enums
{
    /// <summary>
    /// Represents the material status for a job card
    /// </summary>
    public enum MaterialStatus
    {
        Pending,
        Allocated,
        PartiallyAllocated,
        Issued,
        PartiallyIssued,
        Consumed,
        Returned
    }
}
