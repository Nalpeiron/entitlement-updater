namespace EntitlementUpdater.Models;

public sealed class EntitlementListResponse
{
    public required List<Entitlement> Items { get; set; }
}