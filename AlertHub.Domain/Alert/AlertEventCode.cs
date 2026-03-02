using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

public class AlertEventCode
{
    public string ValueName { get; }

    public string Value { get; }

    internal AlertEventCode(string valueName, string value)
    {
        if (string.IsNullOrWhiteSpace(valueName))
            throw new DomainException(AlertDomainErrors.EventCodeValueNameRequired);

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(AlertDomainErrors.EventCodeValueRequired);

        ValueName = valueName;
        Value = value;
    }
}
