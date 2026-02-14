using System.Globalization;
using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

public sealed record AlertReference
{
    public string Sender { get; }
    public string Identifier { get; }
    public DateTimeOffset Sent { get; }

    public AlertReference(string sender, string identifier, DateTimeOffset sent)
    {
        if (string.IsNullOrWhiteSpace(sender))
            throw new DomainException(AlertDomainErrors.SenderRequired);
        
        Alert.EnsureNoCapForbiddenChars(sender, "sender");

        if (string.IsNullOrWhiteSpace(identifier))
            throw new DomainException(AlertDomainErrors.IdentifierRequired);
        
        Alert.EnsureNoCapForbiddenChars(identifier, "identifier");

        Sender = sender;
        Identifier = identifier;
        Sent = sent;
    }

    public static AlertReference Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(new DomainError("alert.reference.invalid_format", "Reference cannot be empty."));

        var parts = value.Split(',');
        if (parts.Length != 3)
            throw new DomainException(new DomainError("alert.reference.invalid_format", "Reference must be in 'sender,identifier,sent' format."));

        var sender = parts[0];
        var identifier = parts[1];
        
        if (!DateTimeOffset.TryParse(parts[2], CultureInfo.InvariantCulture, DateTimeStyles.None, out var sent))
            throw new DomainException(new DomainError("alert.reference.invalid_sent", "Reference sent time is invalid."));

        return new AlertReference(sender, identifier, sent);
    }

    public override string ToString() => $"{Sender},{Identifier},{Sent.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture)}";
}
