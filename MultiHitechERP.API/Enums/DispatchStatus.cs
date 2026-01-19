namespace MultiHitechERP.API.Enums
{
    /// <summary>
    /// Represents the status of a delivery challan
    /// </summary>
    public enum DispatchStatus
    {
        Pending,
        ReadyForDispatch,
        Dispatched,
        InTransit,
        Delivered,
        Cancelled,
        PartiallyDelivered
    }
}
