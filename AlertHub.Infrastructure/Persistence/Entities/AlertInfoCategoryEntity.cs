using AlertHub.Domain.Alert;

namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertInfoCategoryEntity
{
    public Guid AlertInfoId { get; set; }

    public AlertInfoCategory Category { get; set; }

    public AlertInfoEntity Info { get; set; } = null!;
}
