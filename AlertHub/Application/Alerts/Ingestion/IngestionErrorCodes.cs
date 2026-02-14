namespace AlertHub.Application.Alerts.Ingestion;

public static class IngestionErrorCodes
{
    public const string UnsupportedContentType = "ingestion.content_type.unsupported";
    public const string InvalidPayload = "ingestion.payload.invalid";
    public const string XmlSchemaInvalid = "ingestion.xml.schema_invalid";
}
