namespace AlertHub.Infrastructure.Persistence.Entities;

public sealed class AlertInfoResourceEntity
{
    public Guid Id { get; set; }

    public Guid AlertInfoId { get; set; }

    public string ResourceDescription { get; set; } = string.Empty;

    public string? MimeType { get; set; }

    public long? Size { get; set; }

    public string? Uri { get; set; }

    /// <summary>SHA-1 hash of the resource content. derefUri (embedded binary) is intentionally omitted.</summary>
    public string? Digest { get; set; }

    public AlertInfoEntity Info { get; set; } = null!;
}
