namespace MultiHitechERP.API.Enums
{
    /// <summary>
    /// Represents the status of a material requisition
    /// </summary>
    public enum RequisitionStatus
    {
        Pending,
        Approved,
        Rejected,
        PartiallyAllocated,
        Allocated,
        PartiallyIssued,
        Issued,
        Completed,
        Cancelled
    }
}
