using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

public class AlertInfo
{
    public Guid Id { get; } = Guid.NewGuid();

    public string Event { get; }

    public AlertUrgency Urgency { get; }

    public AlertSeverity Severity { get; }

    public AlertCertainty Certainty { get; }

    public string? Language { get; private set; }

    public string? Audience { get; private set; }

    public DateTimeOffset? Effective { get; private set; }

    public DateTimeOffset? Onset { get; private set; }

    public DateTimeOffset? Expires { get; private set; }

    public string? SenderName { get; private set; }

    public string? Headline { get; private set; }

    public string? Description { get; private set; }

    public string? Instruction { get; private set; }

    public string? Web { get; private set; }

    public string? Contact { get; private set; }

    public IReadOnlyCollection<AlertInfoCategory> Categories => _categories;

    public IReadOnlyCollection<AlertResponseType> ResponseTypes => _responseTypes;

    public IReadOnlyCollection<AlertEventCode> EventCodes => _eventCodes;

    public IReadOnlyCollection<AlertParameter> Parameters => _parameters;

    public IReadOnlyCollection<AlertResource> Resources => _resources;

    public IReadOnlyCollection<AlertArea> Areas => _areas;

    private readonly HashSet<AlertInfoCategory> _categories = [];
    private readonly HashSet<AlertResponseType> _responseTypes = [];
    private readonly List<AlertEventCode> _eventCodes = [];
    private readonly List<AlertParameter> _parameters = [];
    private readonly List<AlertResource> _resources = [];
    private readonly List<AlertArea> _areas = [];

    internal AlertInfo(
        string @event,
        AlertUrgency urgency,
        AlertSeverity severity,
        AlertCertainty certainty,
        IEnumerable<AlertInfoCategory> categories,
        IEnumerable<AlertArea> areas)
    {
        if (string.IsNullOrWhiteSpace(@event))
            throw new DomainException(AlertDomainErrors.EventRequired);

        if (!Enum.IsDefined(urgency))
            throw new DomainException(AlertDomainErrors.InvalidUrgency);

        if (!Enum.IsDefined(severity))
            throw new DomainException(AlertDomainErrors.InvalidSeverity);

        if (!Enum.IsDefined(certainty))
            throw new DomainException(AlertDomainErrors.InvalidCertainty);

        Event = @event;
        Urgency = urgency;
        Severity = severity;
        Certainty = certainty;

        foreach (var category in categories)
            AddCategory(category);

        foreach (var area in areas)
            AddArea(area);

        if (_categories.Count == 0)
            throw new DomainException(AlertDomainErrors.CategoryRequired);

    }

    internal void SetLanguage(string? language)
    {
        Language = language;
    }

    internal void SetAudience(string? audience)
    {
        Audience = audience;
    }

    internal void SetEffective(DateTimeOffset? effective)
    {
        Effective = effective;
    }

    internal void SetOnset(DateTimeOffset? onset)
    {
        Onset = onset;
    }

    internal void SetExpires(DateTimeOffset? expires)
    {
        Expires = expires;
    }

    internal void SetSenderName(string? senderName)
    {
        SenderName = senderName;
    }

    internal void SetHeadline(string? headline)
    {
        Headline = headline;
    }

    internal void SetDescription(string? description)
    {
        Description = description;
    }

    internal void SetInstruction(string? instruction)
    {
        Instruction = instruction;
    }

    internal void SetWeb(string? web)
    {
        Web = web;
    }

    internal void SetContact(string? contact)
    {
        Contact = contact;
    }

    internal void AddCategory(AlertInfoCategory category)
    {
        if (!Enum.IsDefined(category))
            throw new DomainException(AlertDomainErrors.InvalidCategory);

        _categories.Add(category);
    }

    internal void AddResponseType(AlertResponseType responseType)
    {
        if (!Enum.IsDefined(responseType))
            throw new DomainException(AlertDomainErrors.InvalidResponseType);

        _responseTypes.Add(responseType);
    }

    internal void AddEventCode(string valueName, string value)
    {
        _eventCodes.Add(new AlertEventCode(valueName, value));
    }

    internal void AddParameter(string valueName, string value)
    {
        _parameters.Add(new AlertParameter(valueName, value));
    }

    internal void AddResource(
        string resourceDescription,
        string? mimeType,
        long? size,
        string? uri,
        string? derefUri,
        string? digest)
    {
        _resources.Add(new AlertResource(resourceDescription, mimeType, size, uri, derefUri, digest));
    }

    internal void AddArea(AlertArea area)
    {
        ArgumentNullException.ThrowIfNull(area);
        _areas.Add(area);
    }

    internal AlertArea GetArea(Guid areaId)
    {
        var area = _areas.FirstOrDefault(a => a.Id == areaId);

        if (area is null)
            throw new DomainException(AlertDomainErrors.AreaNotFound);

        return area;
    }
}
