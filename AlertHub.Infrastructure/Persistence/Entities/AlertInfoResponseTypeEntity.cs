using AlertHub.Domain.Alert;

namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertInfoResponseTypeEntity
{
    public Guid AlertInfoId { get; set; }

    public AlertResponseType ResponseType { get; set; }

    public AlertInfoEntity Info { get; set; } = null!;
}
