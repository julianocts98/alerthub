using AlertHub.Domain.Common;

namespace AlertHub.Domain.Alert;

public static class AlertDomainErrors
{
    public static DomainError IdentifierRequired =>
        new("alert.identifier.required", "Alert identifier is required.");

    public static DomainError SenderRequired =>
        new("alert.sender.required", "Alert sender is required.");

    public static DomainError InvalidStatus =>
        new("alert.status.invalid", "Alert status is invalid.");

    public static DomainError InvalidMessageType =>
        new("alert.msg_type.invalid", "Alert message type is invalid.");

    public static DomainError InvalidScope =>
        new("alert.scope.invalid", "Alert scope is invalid.");

    public static DomainError InfoRequired =>
        new("alert.info.required", "At least one info block is required.");

    public static DomainError InfoNotFound =>
        new("alert.info.not_found", "Info block was not found.");

    public static DomainError EventRequired =>
        new("alert.info.event.required", "Info event is required.");

    public static DomainError InvalidUrgency =>
        new("alert.info.urgency.invalid", "Info urgency is invalid.");

    public static DomainError InvalidSeverity =>
        new("alert.info.severity.invalid", "Info severity is invalid.");

    public static DomainError InvalidCertainty =>
        new("alert.info.certainty.invalid", "Info certainty is invalid.");

    public static DomainError InvalidCategory =>
        new("alert.info.category.invalid", "Info category is invalid.");

    public static DomainError CategoryRequired =>
        new("alert.info.category.required", "At least one info category is required.");

    public static DomainError InvalidResponseType =>
        new("alert.info.response_type.invalid", "Info response type is invalid.");

    public static DomainError EventCodeValueNameRequired =>
        new("alert.info.event_code.value_name.required", "Event code valueName is required.");

    public static DomainError EventCodeValueRequired =>
        new("alert.info.event_code.value.required", "Event code value is required.");

    public static DomainError ParameterValueNameRequired =>
        new("alert.info.parameter.value_name.required", "Parameter valueName is required.");

    public static DomainError ParameterValueRequired =>
        new("alert.info.parameter.value.required", "Parameter value is required.");

    public static DomainError ResourceDescriptionRequired =>
        new("alert.info.resource.description.required", "Resource description is required.");

    public static DomainError ResourceMimeTypeRequired =>
        new("alert.info.resource.mime_type.required", "Resource MIME type is required.");

    public static DomainError ResourceSizeInvalid =>
        new("alert.info.resource.size.invalid", "Resource size cannot be negative.");

    public static DomainError AreaRequired =>
        new("alert.info.area.required", "At least one area is required.");

    public static DomainError AreaNotFound =>
        new("alert.info.area.not_found", "Area was not found.");

    public static DomainError AreaDescriptionRequired =>
        new("alert.info.area.description.required", "Area description is required.");

    public static DomainError AreaDefinitionRequired =>
        new("alert.info.area.definition.required", "Area must have at least one polygon, circle or geocode.");

    public static DomainError AreaPolygonRequired =>
        new("alert.info.area.polygon.required", "Area polygon is required.");

    public static DomainError AreaPolygonInvalid =>
        new("alert.info.area.polygon.invalid", "Area polygon must have at least 4 coordinate pairs and be closed (first and last points must be identical).");

    public static DomainError AreaCircleRequired =>
        new("alert.info.area.circle.required", "Area circle is required.");

    public static DomainError AreaCircleInvalid =>
        new("alert.info.area.circle.invalid", "Area circle must be a coordinate pair and a radius.");

    public static DomainError GeoCodeValueNameRequired =>
        new("alert.info.area.geocode.value_name.required", "Geocode valueName is required.");

    public static DomainError GeoCodeValueRequired =>
        new("alert.info.area.geocode.value.required", "Geocode value is required.");

    public static DomainError AddressRequired =>
        new("alert.address.required", "Address is required.");

    public static DomainError CodeRequired =>
        new("alert.code.required", "Code is required.");

    public static DomainError ReferenceRequired =>
        new("alert.reference.required", "Reference is required.");

    public static DomainError IncidentRequired =>
        new("alert.incident.required", "Incident is required.");

    public static DomainError RestrictionRequiredForRestrictedScope =>
        new("alert.restriction.required", "Restriction is required when scope is Restricted.");

    public static DomainError AddressRequiredForPrivateScope =>
        new("alert.address.required_for_private", "At least one address is required when scope is Private.");

    public static DomainError ReferenceRequiredForUpdateOrCancel =>
        new("alert.reference.required_for_update_cancel", "At least one reference is required when message type is Update or Cancel.");

    public static DomainError InfoOnsetBeforeEffective =>
        new("alert.info.time.onset_before_effective", "Onset cannot be earlier than effective time.");

    public static DomainError InfoExpiresBeforeOnset =>
        new("alert.info.time.expires_before_onset", "Expires cannot be earlier than onset time.");

    public static DomainError InfoExpiresBeforeEffective =>
        new("alert.info.time.expires_before_effective", "Expires cannot be earlier than effective time.");
}
