using AlertHub.Application.Common;
using AlertHub.Domain.Alert;
using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public record CreateSubscriptionRequest(
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
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(ISubscriptionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionResponse> CreateSubscriptionAsync(CreateSubscriptionRequest request, string userId, CancellationToken ct)
    {
        var subscription = Subscription.Create(
            userId,
            request.Channel,
            request.Target,
            request.MinSeverity,
            request.Categories);

        await _repository.AddAsync(subscription, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(subscription);
    }

    public async Task<SubscriptionResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var subscription = await _repository.GetByIdAsync(id, ct);
        return subscription is null ? null : MapToResponse(subscription);
    }

    private static SubscriptionResponse MapToResponse(Subscription s) =>
        new(s.Id, s.UserId, s.Channel, s.Target, s.IsActive, s.MinSeverity, s.Categories.ToList());
}
