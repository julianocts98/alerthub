using AlertHub.Application.Common;
using AlertHub.Domain.Alert;
using AlertHub.Domain.Common;
using AlertHub.Domain.Subscriptions;

namespace AlertHub.Application.Subscriptions;

public sealed class SubscriptionService
{
    private readonly ISubscriptionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(ISubscriptionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SubscriptionResponse>> CreateSubscriptionAsync(CreateSubscriptionRequest request, string userId, CancellationToken ct)
    {
        try
        {
            var subscription = Subscription.Create(
                userId,
                request.Channel,
                request.Target,
                request.MinSeverity,
                request.Categories);

            await _repository.AddAsync(subscription, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<SubscriptionResponse>.Success(MapToResponse(subscription));
        }
        catch (DomainException ex)
        {
            return Result<SubscriptionResponse>.Failure(new ResultError(ex.Error.Code, ex.Error.Message));
        }
    }

    public async Task<Result<SubscriptionResponse>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var subscription = await _repository.GetByIdAsync(id, ct);
        if (subscription is null)
        {
            return Result<SubscriptionResponse>.Failure(
                new ResultError("subscription.not_found", $"Subscription with ID '{id}' was not found."));
        }

        return Result<SubscriptionResponse>.Success(MapToResponse(subscription));
    }

    private static SubscriptionResponse MapToResponse(Subscription s) =>
        new(s.Id, s.UserId, s.Channel, s.Target, s.IsActive, s.MinSeverity, s.Categories.ToList());
}
