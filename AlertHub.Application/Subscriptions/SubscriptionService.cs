using AlertHub.Domain.Common;
using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _repository;
    private readonly ISubscriptionQueries _queries;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(
        ISubscriptionRepository repository,
        ISubscriptionQueries queries,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _queries = queries;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SubscriptionResponse>> CreateSubscriptionAsync(CreateSubscriptionRequest request, string userId, CancellationToken ct)
    {
        var subscriptionResult = Subscription.Create(
            userId,
            request.Channel,
            request.Target,
            request.MinSeverity,
            request.Categories);
            
        if (!subscriptionResult.IsSuccess)
        {
            return Result<SubscriptionResponse>.Failure(subscriptionResult.Error!);
        }

        var subscription = subscriptionResult.Value!;

        await _repository.AddAsync(subscription, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<SubscriptionResponse>.Success(MapToResponse(subscription));
    }

    public async Task<Result<SubscriptionResponse>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var subscription = await _queries.GetByIdAsync(id, ct);
        if (subscription is null)
        {
            return Result<SubscriptionResponse>.Failure(
                ResultError.NotFound(SubscriptionErrorCodes.NotFound, $"Subscription with ID '{id}' was not found."));
        }

        return Result<SubscriptionResponse>.Success(subscription);
    }

    private static SubscriptionResponse MapToResponse(Subscription s) =>
        new(s.Id, s.UserId, s.Channel, s.Target, s.IsActive, s.MinSeverity, s.Categories.ToList());
}
