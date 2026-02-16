using System.Xml;
using System.Xml.Schema;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Common;

namespace AlertHub.Infrastructure.Alerts.Ingestion;

public sealed class CapXmlSchemaValidator : ICapXmlSchemaValidator
{
    private const string CapNamespace = "urn:oasis:names:tc:emergency:cap:1.2";
    private const string SchemaRelativePath = "Infrastructure/Alerts/Ingestion/Schemas/cap1_2.xsd";

    private readonly XmlSchemaSet _schemas;

    public CapXmlSchemaValidator(IHostEnvironment environment)
    {
        var schemaPath = Path.Combine(environment.ContentRootPath, SchemaRelativePath);
        _schemas = new XmlSchemaSet();
        _schemas.Add(CapNamespace, schemaPath);
    }

    public Result Validate(string rawXml)
    {
        var errors = new List<string>();

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = _schemas,
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        };

        settings.ValidationEventHandler += (_, args) =>
        {
            var ex = args.Exception;
            var location = ex is null ? string.Empty : $"(line {ex.LineNumber}, pos {ex.LinePosition}) ";
            errors.Add($"{location}{args.Message}");
        };

        try
        {
            using var textReader = new StringReader(rawXml);
            using var xmlReader = XmlReader.Create(textReader, settings);
            while (xmlReader.Read())
            {
            }
        }
        catch (XmlException ex)
        {
            errors.Add($"(line {ex.LineNumber}, pos {ex.LinePosition}) {ex.Message}");
        }

        if (errors.Count == 0)
            return Result.Success();

        var joinedErrors = string.Join(" | ", errors);
        return Result.Failure(new ResultError(IngestionErrorCodes.XmlSchemaInvalid, joinedErrors));
    }
}
