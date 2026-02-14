using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

public class AlertGeoCode
{
    public string ValueName { get; }

    public string Value { get; }

    internal AlertGeoCode(string valueName, string value)
    {
        if (string.IsNullOrWhiteSpace(valueName))
            throw new DomainException(AlertDomainErrors.GeoCodeValueNameRequired);

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(AlertDomainErrors.GeoCodeValueRequired);

        ValueName = valueName;
        Value = value;
    }
}
