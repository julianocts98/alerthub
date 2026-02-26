using AlertHub.Domain.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public interface ICapXmlSchemaValidator
{
    Result Validate(string rawXml);
}
