using AlertHub.Application.Common;
using AlertHub.Domain.Common;
using DomainAlert = AlertHub.Domain.Alert.Alert;

namespace AlertHub.Application.Alerts.Ingestion;

public sealed class AlertFactory
{
    public Task<Result<DomainAlert>> CreateAsync(AlertIngestionRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            var alert = DomainAlert.Create(
                identifier: request.Identifier,
                sender: request.Sender,
                sent: request.Sent,
                status: request.Status,
                messageType: request.MessageType,
                scope: request.Scope);

            alert.SetSource(request.Source);
            alert.SetRestriction(request.Restriction);
            alert.SetNote(request.Note);

            foreach (var address in SplitCapValueList(request.Addresses))
                alert.AddAddress(address);

            foreach (var code in request.Codes ?? [])
                alert.AddCode(code);

            foreach (var reference in SplitCapValueList(request.References))
                alert.AddReference(reference);

            foreach (var incident in SplitCapValueList(request.Incidents))
                alert.AddIncident(incident);

            foreach (var info in request.Infos ?? [])
            {
                var infoId = alert.AddInfo(
                    @event: info.Event,
                    urgency: info.Urgency,
                    severity: info.Severity,
                    certainty: info.Certainty,
                    categories: info.Categories ?? [],
                    areaDescriptions: (info.Areas ?? []).Select(a => a.AreaDescription),
                    language: info.Language,
                    audience: info.Audience,
                    effective: info.Effective,
                    onset: info.Onset,
                    expires: info.Expires,
                    senderName: info.SenderName,
                    headline: info.Headline,
                    description: info.Description,
                    instruction: info.Instruction,
                    web: info.Web,
                    contact: info.Contact);

                foreach (var responseType in info.ResponseTypes ?? [])
                    alert.AddInfoResponseType(infoId, responseType);

                foreach (var eventCode in info.EventCodes ?? [])
                    alert.AddInfoEventCode(infoId, eventCode.ValueName, eventCode.Value);

                foreach (var parameter in info.Parameters ?? [])
                    alert.AddInfoParameter(infoId, parameter.ValueName, parameter.Value);

                foreach (var resource in info.Resources ?? [])
                {
                    alert.AddInfoResource(
                        infoId: infoId,
                        resourceDescription: resource.ResourceDescription,
                        mimeType: resource.MimeType,
                        size: resource.Size,
                        uri: resource.Uri,
                        derefUri: resource.DerefUri,
                        digest: resource.Digest);
                }

                var domainInfo = alert.Infos.Single(i => i.Id == infoId);
                var domainAreas = domainInfo.Areas.ToList();
                var requestAreas = info.Areas ?? [];

                for (var i = 0; i < requestAreas.Count && i < domainAreas.Count; i++)
                {
                    var area = requestAreas[i];
                    var areaId = domainAreas[i].Id;

                    foreach (var polygon in area.Polygons ?? [])
                        alert.AddInfoAreaPolygon(infoId, areaId, polygon);

                    foreach (var circle in area.Circles ?? [])
                        alert.AddInfoAreaCircle(infoId, areaId, circle);

                    foreach (var geoCode in area.GeoCodes ?? [])
                        alert.AddInfoAreaGeoCode(infoId, areaId, geoCode.ValueName, geoCode.Value);

                    alert.SetInfoAreaAltitude(infoId, areaId, area.Altitude);
                    alert.SetInfoAreaCeiling(infoId, areaId, area.Ceiling);
                }
            }

            alert.ValidateForPublication();

            return Task.FromResult(Result<DomainAlert>.Success(alert));
        }
        catch (DomainException ex)
        {
            var error = new ResultError(ex.Error.Code, ex.Error.Message);
            return Task.FromResult(Result<DomainAlert>.Failure(error));
        }
    }

    private static IEnumerable<string> SplitCapValueList(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return [];

        return raw
            .Split((char[])null!, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(v => !string.IsNullOrWhiteSpace(v));
    }
}
