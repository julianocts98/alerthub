using AlertHub.Domain.Alert.Events;
using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

/// <summary>
/// CAP alert aggregate root.
/// All write operations are coordinated through this root.
/// </summary>
public class Alert : AggregateRoot
{
    public Guid Id { get; }

    public string Identifier { get; }

    public string Sender { get; }

    public DateTimeOffset Sent { get; }

    public AlertStatus Status { get; }

    public AlertMessageType MessageType { get; }

    public AlertScope Scope { get; }

    public string? Source { get; private set; }

    public string? Restriction { get; private set; }

    public string? Note { get; private set; }

    public IReadOnlyCollection<string> Addresses => _addresses;

    public IReadOnlyCollection<string> Codes => _codes;

    public IReadOnlyCollection<AlertReference> References => _references;

    public IReadOnlyCollection<string> Incidents => _incidents;

    public IReadOnlyCollection<AlertInfo> Infos => _infos;

    private readonly List<string> _addresses = [];
    private readonly List<string> _codes = [];
    private readonly List<AlertReference> _references = [];
    private readonly List<string> _incidents = [];
    private readonly List<AlertInfo> _infos = [];

    private Alert(
        Guid id,
        string identifier,
        string sender,
        DateTimeOffset sent,
        AlertStatus status,
        AlertMessageType messageType,
        AlertScope scope)
    {
        Id = id;
        Identifier = identifier;
        Sender = sender;
        Sent = sent;
        Status = status;
        MessageType = messageType;
        Scope = scope;
    }

    public static Alert Create(
        string identifier,
        string sender,
        DateTimeOffset sent,
        AlertStatus status,
        AlertMessageType messageType,
        AlertScope scope)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new DomainException(AlertDomainErrors.IdentifierRequired);

        EnsureNoCapForbiddenChars(identifier, "identifier");

        if (string.IsNullOrWhiteSpace(sender))
            throw new DomainException(AlertDomainErrors.SenderRequired);

        EnsureNoCapForbiddenChars(sender, "sender");

        if (!Enum.IsDefined(status))
            throw new DomainException(AlertDomainErrors.InvalidStatus);

        if (!Enum.IsDefined(messageType))
            throw new DomainException(AlertDomainErrors.InvalidMessageType);

        if (!Enum.IsDefined(scope))
            throw new DomainException(AlertDomainErrors.InvalidScope);

        var alert = new Alert(Guid.NewGuid(), identifier, sender, sent, status, messageType, scope);

        alert.RaiseDomainEvent(new AlertIngestedDomainEvent(alert.Id, alert.Identifier, alert.Sender, alert.Sent));

        return alert;
    }

    public void SetSource(string? source)
    {
        Source = source;
    }

    public void SetRestriction(string? restriction)
    {
        Restriction = restriction;
    }

    public void SetNote(string? note)
    {
        Note = note;
    }

    public void AddAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException(AlertDomainErrors.AddressRequired);

        _addresses.Add(address);
    }

    public void AddCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException(AlertDomainErrors.CodeRequired);

        _codes.Add(code);
    }

    public void AddReference(string sender, string identifier, DateTimeOffset sent)
    {
        _references.Add(new AlertReference(sender, identifier, sent));
    }

    public void AddReference(string reference)
    {
        _references.Add(AlertReference.Parse(reference));
    }

    public void AddIncident(string incident)
    {
        if (string.IsNullOrWhiteSpace(incident))
            throw new DomainException(AlertDomainErrors.IncidentRequired);

        _incidents.Add(incident);
    }

    public Guid AddInfo(
        string @event,
        AlertUrgency urgency,
        AlertSeverity severity,
        AlertCertainty certainty,
        IEnumerable<AlertInfoCategory> categories,
        IEnumerable<string> areaDescriptions,
        string? language = null,
        string? audience = null,
        DateTimeOffset? effective = null,
        DateTimeOffset? onset = null,
        DateTimeOffset? expires = null,
        string? senderName = null,
        string? headline = null,
        string? description = null,
        string? instruction = null,
        string? web = null,
        string? contact = null)
    {
        var areaList = areaDescriptions
            .Select(d => new AlertArea(d))
            .ToList();

        var info = new AlertInfo(@event, urgency, severity, certainty, categories, areaList);

        info.SetLanguage(language);
        info.SetAudience(audience);
        info.SetEffective(effective);
        info.SetOnset(onset);
        info.SetExpires(expires);
        info.SetSenderName(senderName);
        info.SetHeadline(headline);
        info.SetDescription(description);
        info.SetInstruction(instruction);
        info.SetWeb(web);
        info.SetContact(contact);

        _infos.Add(info);

        return info.Id;
    }

    public void AddInfoCategory(Guid infoId, AlertInfoCategory category)
    {
        GetInfo(infoId).AddCategory(category);
    }

    public void AddInfoResponseType(Guid infoId, AlertResponseType responseType)
    {
        GetInfo(infoId).AddResponseType(responseType);
    }

    public void AddInfoEventCode(Guid infoId, string valueName, string value)
    {
        GetInfo(infoId).AddEventCode(valueName, value);
    }

    public void AddInfoParameter(Guid infoId, string valueName, string value)
    {
        GetInfo(infoId).AddParameter(valueName, value);
    }

    public void AddInfoResource(
        Guid infoId,
        string resourceDescription,
        string? mimeType,
        long? size = null,
        string? uri = null,
        string? derefUri = null,
        string? digest = null)
    {
        GetInfo(infoId).AddResource(resourceDescription, mimeType, size, uri, derefUri, digest);
    }

    public Guid AddInfoArea(Guid infoId, string areaDescription)
    {
        var area = new AlertArea(areaDescription);
        GetInfo(infoId).AddArea(area);
        return area.Id;
    }

    public void AddInfoAreaPolygon(Guid infoId, Guid areaId, string polygon)
    {
        var polygonVo = Common.Geometry.Polygon.Parse(polygon);
        GetInfo(infoId).GetArea(areaId).AddPolygon(polygonVo);
    }

    public void AddInfoAreaCircle(Guid infoId, Guid areaId, string circle)
    {
        var circleVo = Common.Geometry.Circle.Parse(circle);
        GetInfo(infoId).GetArea(areaId).AddCircle(circleVo);
    }

    public void AddInfoAreaGeoCode(Guid infoId, Guid areaId, string valueName, string value)
    {
        GetInfo(infoId).GetArea(areaId).AddGeoCode(valueName, value);
    }

    public void SetInfoAreaAltitude(Guid infoId, Guid areaId, double? altitude)
    {
        GetInfo(infoId).GetArea(areaId).SetAltitude(altitude);
    }

    public void SetInfoAreaCeiling(Guid infoId, Guid areaId, double? ceiling)
    {
        GetInfo(infoId).GetArea(areaId).SetCeiling(ceiling);
    }

    public void ValidateForPublication()
    {
        if (Scope == AlertScope.Restricted && string.IsNullOrWhiteSpace(Restriction))
            throw new DomainException(AlertDomainErrors.RestrictionRequiredForRestrictedScope);

        if (Scope == AlertScope.Private && _addresses.Count == 0)
            throw new DomainException(AlertDomainErrors.AddressRequiredForPrivateScope);

        if ((MessageType == AlertMessageType.Update || MessageType == AlertMessageType.Cancel) && _references.Count == 0)
            throw new DomainException(AlertDomainErrors.ReferenceRequiredForUpdateOrCancel);

        foreach (var info in _infos)
        {
            if (info.Categories.Count == 0)
                throw new DomainException(AlertDomainErrors.CategoryRequired);

            if (info.Areas.Count == 0)
                throw new DomainException(AlertDomainErrors.AreaRequired);

            if (info.Effective.HasValue && info.Onset.HasValue && info.Onset.Value < info.Effective.Value)
                throw new DomainException(AlertDomainErrors.InfoOnsetBeforeEffective);

            if (info.Onset.HasValue && info.Expires.HasValue && info.Expires.Value < info.Onset.Value)
                throw new DomainException(AlertDomainErrors.InfoExpiresBeforeOnset);

            if (!info.Onset.HasValue && info.Effective.HasValue && info.Expires.HasValue && info.Expires.Value < info.Effective.Value)
                throw new DomainException(AlertDomainErrors.InfoExpiresBeforeEffective);
        }
    }

    private AlertInfo GetInfo(Guid infoId)
    {
        var info = _infos.FirstOrDefault(i => i.Id == infoId);

        if (info is null)
            throw new DomainException(AlertDomainErrors.InfoNotFound);

        return info;
    }

    internal static void EnsureNoCapForbiddenChars(string value, string fieldName)
    {
        if (value.Any(c => c is ' ' or ',' or '<' or '&'))
            throw new DomainException(new DomainError($"alert.{fieldName}.invalid_characters", $"{fieldName} cannot contain spaces, commas, < or &."));
    }
}
