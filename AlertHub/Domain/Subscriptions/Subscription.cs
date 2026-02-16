using AlertHub.Domain.Alert;
using AlertHub.Domain.Common;
using AlertHub.Domain.Subscriptions.Events;

namespace AlertHub.Domain.Subscriptions;

public sealed class Subscription : AggregateRoot
{
    public Guid Id { get; }

    public string UserId { get; }

    public SubscriptionChannel Channel { get; }

    public string Target { get; }

    public bool IsActive { get; private set; }

    public AlertSeverity? MinSeverity { get; private set; }

    public IReadOnlyCollection<AlertInfoCategory> Categories => _categories;
    private readonly List<AlertInfoCategory> _categories = [];

    private Subscription(
        Guid id,
        string userId,
        SubscriptionChannel channel,
        string target,
        bool isActive)
    {
        Id = id;
        UserId = userId;
        Channel = channel;
        Target = target;
        IsActive = isActive;
    }

    internal Subscription(
        Guid id,
        string userId,
        SubscriptionChannel channel,
        string target,
        bool isActive,
        AlertSeverity? minSeverity,
        IEnumerable<AlertInfoCategory> categories)
        : this(id, userId, channel, target, isActive)
    {
        MinSeverity = minSeverity;
        _categories.AddRange(categories);
    }

    public static Subscription Create(
        string userId,
        SubscriptionChannel channel,
        string target,
        AlertSeverity? minSeverity = null,
        IEnumerable<AlertInfoCategory>? categories = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException(new DomainError("subscription.user_id.required", "User ID is required."));

        if (string.IsNullOrWhiteSpace(target))
            throw new DomainException(new DomainError("subscription.target.required", "Target is required."));

        // Basic validation for target based on channel
        if (channel == SubscriptionChannel.Sms && !IsValidPhoneNumber(target))
            throw new DomainException(new DomainError("subscription.target.invalid_sms", "Invalid phone number for SMS channel."));

        if (channel == SubscriptionChannel.Email && !IsValidEmail(target))
            throw new DomainException(new DomainError("subscription.target.invalid_email", "Invalid email address for Email channel."));

        var subscription = new Subscription(Guid.NewGuid(), userId, channel, target, true);

        subscription.UpdateFilter(minSeverity, categories);

        subscription.RaiseDomainEvent(new SubscriptionCreatedDomainEvent(subscription.Id, subscription.UserId, subscription.Target));

        return subscription;
    }

    public void UpdateFilter(AlertSeverity? minSeverity, IEnumerable<AlertInfoCategory>? categories)
    {
        MinSeverity = minSeverity;
        _categories.Clear();
        if (categories != null)
        {
            _categories.AddRange(categories.Distinct());
        }
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    private static bool IsValidPhoneNumber(string phone)
    {
        // Simple check: start with +, then digits, length between 7 and 15
        return phone.Length >= 7 && phone.Length <= 15 && phone.All(c => char.IsDigit(c) || c == '+');
    }

    private static bool IsValidEmail(string email)
    {
        return email.Contains('@') && email.Contains('.');
    }
}
