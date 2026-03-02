namespace AlertHub.Application.Deliveries;

public sealed class GetDeliveriesRequest
{
    public DeliveryStatusFilter? Status { get; init; }

    public int Limit { get; init; } = 50;
}
