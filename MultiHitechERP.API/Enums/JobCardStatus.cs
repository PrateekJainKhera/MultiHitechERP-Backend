namespace MultiHitechERP.API.Enums
{
    /// <summary>
    /// Represents the status of a job card
    /// </summary>
    public enum JobCardStatus
    {
        Pending,
        PendingMaterial,
        Ready,
        InProgress,
        Paused,
        Completed,
        Blocked,
        Cancelled,
        Rework
    }
}
