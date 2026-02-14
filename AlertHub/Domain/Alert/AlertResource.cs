using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

public class AlertResource
{
    public string ResourceDescription { get; }

    public string? MimeType { get; }

    public long? Size { get; }

    public string? Uri { get; }

    public string? DerefUri { get; }

    public string? Digest { get; }

    internal AlertResource(
        string resourceDescription,
        string? mimeType,
        long? size,
        string? uri,
        string? derefUri,
        string? digest)
    {
        if (string.IsNullOrWhiteSpace(resourceDescription))
            throw new DomainException(AlertDomainErrors.ResourceDescriptionRequired);

        if (size is < 0)
            throw new DomainException(AlertDomainErrors.ResourceSizeInvalid);

        ResourceDescription = resourceDescription;
        MimeType = mimeType;
        Size = size;
        Uri = uri;
        DerefUri = derefUri;
        Digest = digest;
    }
}
