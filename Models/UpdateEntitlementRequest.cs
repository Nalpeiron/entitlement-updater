namespace EntitlementUpdater.Models;

public sealed class UpdateEntitlementRequest
{
    public required IntervalType LeasePeriod { get; set; }
}