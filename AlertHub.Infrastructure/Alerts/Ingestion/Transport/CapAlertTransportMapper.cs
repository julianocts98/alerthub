using AlertHub.Application.Alerts.Ingestion;

namespace AlertHub.Infrastructure.Alerts.Ingestion.Transport;

public static class CapAlertTransportMapper
{
    public static AlertIngestionRequest ToApplicationRequest(CapAlertTransportRequest source)
    {
        return new AlertIngestionRequest
        {
            Identifier = source.Identifier,
            Sender = source.Sender,
            Sent = source.Sent,
            Status = source.Status,
            MessageType = source.MessageType,
            Scope = source.Scope,
            Source = source.Source,
            Restriction = source.Restriction,
            Note = source.Note,
            Addresses = source.Addresses,
            Codes = source.Codes ?? [],
            References = source.References,
            Incidents = source.Incidents,
            Infos = (source.Infos ?? [])
                .Select(info => new AlertInfoRequest
                {
                    Language = info.Language,
                    Categories = info.Categories ?? [],
                    Event = info.Event,
                    ResponseTypes = info.ResponseTypes ?? [],
                    Urgency = info.Urgency,
                    Severity = info.Severity,
                    Certainty = info.Certainty,
                    Audience = info.Audience,
                    EventCodes = (info.EventCodes ?? [])
                        .Select(kv => new AlertKeyValueRequest
                        {
                            ValueName = kv.ValueName,
                            Value = kv.Value
                        })
                        .ToList(),
                    Effective = info.Effective,
                    Onset = info.Onset,
                    Expires = info.Expires,
                    SenderName = info.SenderName,
                    Headline = info.Headline,
                    Description = info.Description,
                    Instruction = info.Instruction,
                    Web = info.Web,
                    Contact = info.Contact,
                    Parameters = (info.Parameters ?? [])
                        .Select(kv => new AlertKeyValueRequest
                        {
                            ValueName = kv.ValueName,
                            Value = kv.Value
                        })
                        .ToList(),
                    Resources = (info.Resources ?? [])
                        .Select(resource => new AlertResourceRequest
                        {
                            ResourceDescription = resource.ResourceDescription,
                            MimeType = resource.MimeType,
                            Size = resource.Size,
                            Uri = resource.Uri,
                            DerefUri = resource.DerefUri,
                            Digest = resource.Digest
                        })
                        .ToList(),
                    Areas = (info.Areas ?? [])
                        .Select(area => new AlertAreaRequest
                        {
                            AreaDescription = area.AreaDescription,
                            Polygons = area.Polygons ?? [],
                            Circles = area.Circles ?? [],
                            GeoCodes = (area.GeoCodes ?? [])
                                .Select(kv => new AlertKeyValueRequest
                                {
                                    ValueName = kv.ValueName,
                                    Value = kv.Value
                                })
                                .ToList(),
                            Altitude = area.Altitude,
                            Ceiling = area.Ceiling
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}
