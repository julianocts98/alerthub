using AlertHub.Domain.Alert;
using AlertHub.Domain.Common;

namespace AlertHub.Tests.Domain.Alert;

public class AlertCap12ComplianceTests
{
    [Fact]
    public void ValidateForPublication_WithoutInfos_ShouldBeAllowedByCap12()
    {
        var alert = AlertFaker.CreateAlert();

        alert.ValidateForPublication();
    }

    [Fact]
    public void AddInfo_WithoutAreas_ShouldBeAllowedByCap12()
    {
        var alert = AlertFaker.CreateAlert();

        alert.AddInfo(
            @event: AlertFaker.NextEvent(),
            urgency: AlertUrgency.Immediate,
            severity: AlertSeverity.Severe,
            certainty: AlertCertainty.Observed,
            categories: [AlertInfoCategory.Safety],
            areaDescriptions: []);

        alert.ValidateForPublication();
    }

    [Fact]
    public void ValidateForPublication_WithAreaWithoutPolygonCircleGeoCode_ShouldBeAllowedByCap12()
    {
        var alert = AlertFaker.CreateAlert();

        alert.AddInfo(
            @event: AlertFaker.NextEvent(),
            urgency: AlertUrgency.Immediate,
            severity: AlertSeverity.Severe,
            certainty: AlertCertainty.Observed,
            categories: [AlertInfoCategory.Safety],
            areaDescriptions: [AlertFaker.NextAreaDescription()]);

        alert.ValidateForPublication();
    }

    [Fact]
    public void AddInfoResource_WithNullMimeType_ShouldBeAllowedByCap12()
    {
        var alert = AlertFaker.CreateAlert();

        var infoId = alert.AddInfo(
            @event: AlertFaker.NextEvent(),
            urgency: AlertUrgency.Immediate,
            severity: AlertSeverity.Severe,
            certainty: AlertCertainty.Observed,
            categories: [AlertInfoCategory.Safety],
            areaDescriptions: []);

        alert.AddInfoResource(
            infoId,
            resourceDescription: AlertFaker.NextResourceDescription(),
            mimeType: null);
    }

    [Fact]
    public void AddInfoAreaCircle_WithZeroRadius_ShouldThrowDomainException()
    {
        var alert = AlertFaker.CreateAlert();

        var infoId = alert.AddInfo(
            @event: AlertFaker.NextEvent(),
            urgency: AlertUrgency.Immediate,
            severity: AlertSeverity.Severe,
            certainty: AlertCertainty.Observed,
            categories: [AlertInfoCategory.Safety],
            areaDescriptions: [AlertFaker.NextAreaDescription()]);

        var areaId = alert.Infos.Single().Areas.Single().Id;

        Assert.Throws<DomainException>(() =>
        {
            alert.AddInfoAreaCircle(infoId, areaId, "10,10 0");
        });
    }

    [Theory]
    [InlineData("sender with-space", "id")]
    [InlineData("sender,id", "id")]
    [InlineData("sender<&", "id")]
    [InlineData("sender", "id with-space")]
    [InlineData("sender", "id,comma")]
    [InlineData("sender", "id<&")]
    public void AlertReference_IdentifierAndSenderMustFollowCapCharacterRules(string sender, string identifier)
    {
        Assert.Throws<DomainException>(() =>
        {
            _ = new AlertReference(sender, identifier, DateTimeOffset.UtcNow);
        });
    }

    [Fact]
    public void ValidateForPublication_UpdateWithoutReferences_ShouldThrowDomainException()
    {
        var alert = AlertFaker.CreateAlert(messageType: AlertMessageType.Update);

        Assert.Throws<DomainException>(() =>
        {
            alert.ValidateForPublication();
        });
    }
}
