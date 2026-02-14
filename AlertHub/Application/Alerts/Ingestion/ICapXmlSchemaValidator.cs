using AlertHub.Application.Common;

namespace AlertHub.Application.Alerts.Ingestion;

public interface ICapXmlSchemaValidator
{
    Result Validate(string rawXml);
}
