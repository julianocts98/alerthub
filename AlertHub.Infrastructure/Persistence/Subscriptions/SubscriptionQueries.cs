using AlertHub.Application.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure.Persistence.Subscriptions;

public sealed class SubscriptionQueries : ISubscriptionQueries
{
    private readonly AppDbContext _dbContext;

    public SubscriptionQueries(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Subscriptions
            .AsNoTracking()
            .Include(s => s.Categories)
            .Where(s => s.Id == id)
            .Select(s => new SubscriptionResponse(
                s.Id,
                s.UserId,
                s.Channel,
                s.Target,
                s.IsActive,
                s.MinSeverity,
                s.Categories.Select(c => c.Category).ToList()))
            .FirstOrDefaultAsync(ct);
    }
}
