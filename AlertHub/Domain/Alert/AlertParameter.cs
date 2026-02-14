using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

public class AlertParameter
{
    public string ValueName { get; }

    public string Value { get; }

    internal AlertParameter(string valueName, string value)
    {
        if (string.IsNullOrWhiteSpace(valueName))
            throw new DomainException(AlertDomainErrors.ParameterValueNameRequired);

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(AlertDomainErrors.ParameterValueRequired);

        ValueName = valueName;
        Value = value;
    }
}
