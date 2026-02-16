using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public record CreateSubscriptionRequest(
    string UserId,
    SubscriptionChannel Channel,
    string Target,
    AlertSeverity? MinSeverity = null,
    List<AlertInfoCategory>? Categories = null);

public record SubscriptionResponse(
    Guid Id,
    string UserId,
    SubscriptionChannel Channel,
    string Target,
    bool IsActive,
    AlertSeverity? MinSeverity,
    List<AlertInfoCategory> Categories);

public sealed class SubscriptionService
{
    private readonly ISubscriptionRepository _repository;

    public SubscriptionService(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscriptionResponse> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken ct)
    {
        var subscription = Subscription.Create(
            request.UserId,
            request.Channel,
            request.Target,
            request.MinSeverity,
            request.Categories);

        await _repository.AddAsync(subscription, ct);

        return MapToResponse(subscription);
    }

    private static SubscriptionResponse MapToResponse(Subscription s) =>
        new(s.Id, s.UserId, s.Channel, s.Target, s.IsActive, s.MinSeverity, s.Categories.ToList());
}
