using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using AlertHub.Domain.Alert;

namespace AlertHub.Infrastructure.Alerts.Ingestion.Transport;

[XmlType(Namespace = CapNamespace)]
[XmlRoot("alert", Namespace = CapNamespace)]
public sealed class CapAlertTransportRequest
{
    public const string CapNamespace = "urn:oasis:names:tc:emergency:cap:1.2";

    [Required]
    [MaxLength(200)]
    [RegularExpression(@"^[^ ,<&]+$")]
    [XmlElement("identifier")]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [RegularExpression(@"^[^ ,<&]+$")]
    [XmlElement("sender")]
    public string Sender { get; set; } = string.Empty;

    [Required]
    [XmlElement("sent")]
    public DateTimeOffset Sent { get; set; }

    [Required]
    [EnumDataType(typeof(AlertStatus))]
    [XmlElement("status")]
    public AlertStatus Status { get; set; }

    [Required]
    [EnumDataType(typeof(AlertMessageType))]
    [XmlElement("msgType")]
    public AlertMessageType MessageType { get; set; }

    [Required]
    [EnumDataType(typeof(AlertScope))]
    [XmlElement("scope")]
    public AlertScope Scope { get; set; }

    [MaxLength(500)]
    [XmlElement("source")]
    public string? Source { get; set; }

    [MaxLength(1000)]
    [XmlElement("restriction")]
    public string? Restriction { get; set; }

    [MaxLength(4000)]
    [XmlElement("note")]
    public string? Note { get; set; }

    [XmlElement("addresses")]
    public string? Addresses { get; set; }

    [XmlElement("code")]
    public List<string> Codes { get; set; } = [];

    [XmlElement("references")]
    public string? References { get; set; }

    [XmlElement("incidents")]
    public string? Incidents { get; set; }

    [XmlElement("info")]
    public List<CapAlertInfoTransport> Infos { get; set; } = [];

    [XmlAnyElement]
    public XmlElement[] Any { get; set; } = [];

    [JsonExtensionData]
    [XmlIgnore]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

[XmlType(Namespace = CapAlertTransportRequest.CapNamespace)]
public sealed class CapAlertInfoTransport
{
    [XmlElement("language")]
    [MaxLength(20)]
    public string? Language { get; set; }

    [XmlElement("category")]
    [MinLength(1)]
    public List<AlertInfoCategory> Categories { get; set; } = [];

    [Required]
    [MaxLength(300)]
    [XmlElement("event")]
    public string Event { get; set; } = string.Empty;

    [XmlElement("responseType")]
    public List<AlertResponseType> ResponseTypes { get; set; } = [];

    [Required]
    [EnumDataType(typeof(AlertUrgency))]
    [XmlElement("urgency")]
    public AlertUrgency Urgency { get; set; }

    [Required]
    [EnumDataType(typeof(AlertSeverity))]
    [XmlElement("severity")]
    public AlertSeverity Severity { get; set; }

    [Required]
    [EnumDataType(typeof(AlertCertainty))]
    [XmlElement("certainty")]
    public AlertCertainty Certainty { get; set; }

    [XmlElement("audience")]
    [MaxLength(200)]
    public string? Audience { get; set; }

    [XmlElement("eventCode")]
    public List<CapAlertKeyValueTransport> EventCodes { get; set; } = [];

    [XmlElement("effective")]
    public DateTimeOffset? Effective { get; set; }

    [XmlElement("onset")]
    public DateTimeOffset? Onset { get; set; }

    [XmlElement("expires")]
    public DateTimeOffset? Expires { get; set; }

    [MaxLength(200)]
    [XmlElement("senderName")]
    public string? SenderName { get; set; }

    [MaxLength(300)]
    [XmlElement("headline")]
    public string? Headline { get; set; }

    [MaxLength(4000)]
    [XmlElement("description")]
    public string? Description { get; set; }

    [MaxLength(4000)]
    [XmlElement("instruction")]
    public string? Instruction { get; set; }

    [MaxLength(500)]
    [XmlElement("web")]
    public string? Web { get; set; }

    [MaxLength(300)]
    [XmlElement("contact")]
    public string? Contact { get; set; }

    [XmlElement("parameter")]
    public List<CapAlertKeyValueTransport> Parameters { get; set; } = [];

    [XmlElement("resource")]
    public List<CapAlertResourceTransport> Resources { get; set; } = [];

    [XmlElement("area")]
    public List<CapAlertAreaTransport> Areas { get; set; } = [];
}

[XmlType(Namespace = CapAlertTransportRequest.CapNamespace)]
public sealed class CapAlertKeyValueTransport
{
    [Required]
    [XmlElement("valueName")]
    public string ValueName { get; set; } = string.Empty;

    [Required]
    [XmlElement("value")]
    public string Value { get; set; } = string.Empty;
}

[XmlType(Namespace = CapAlertTransportRequest.CapNamespace)]
public sealed class CapAlertResourceTransport
{
    [Required]
    [XmlElement("resourceDesc")]
    public string ResourceDescription { get; set; } = string.Empty;

    [Required]
    [XmlElement("mimeType")]
    public string MimeType { get; set; } = string.Empty;

    [XmlElement("size")]
    public long? Size { get; set; }

    [XmlElement("uri")]
    public string? Uri { get; set; }

    [XmlElement("derefUri")]
    public string? DerefUri { get; set; }

    [XmlElement("digest")]
    public string? Digest { get; set; }
}

[XmlType(Namespace = CapAlertTransportRequest.CapNamespace)]
public sealed class CapAlertAreaTransport
{
    [Required]
    [XmlElement("areaDesc")]
    public string AreaDescription { get; set; } = string.Empty;

    [XmlElement("polygon")]
    public List<string> Polygons { get; set; } = [];

    [XmlElement("circle")]
    public List<string> Circles { get; set; } = [];

    [XmlElement("geocode")]
    public List<CapAlertKeyValueTransport> GeoCodes { get; set; } = [];

    [XmlElement("altitude")]
    public double? Altitude { get; set; }

    [XmlElement("ceiling")]
    public double? Ceiling { get; set; }
}
