using System.Xml.Serialization;
using AlertHub.Infrastructure.Alerts.Ingestion.Transport;
using AlertHub.Domain.Alert;

namespace AlertHub.Tests.Application.Alerts.Ingestion;

public class AlertIngestionRequestXmlTests
{
    [Fact]
    public void Deserialize_MinimalCap12Xml_MapsExpectedFields()
    {
        const string xml = """
                           <alert xmlns="urn:oasis:names:tc:emergency:cap:1.2">
                             <identifier>cap-alert-123</identifier>
                             <sender>alerts@example.com</sender>
                             <sent>2026-02-14T12:00:00+00:00</sent>
                             <status>Actual</status>
                             <msgType>Alert</msgType>
                             <scope>Public</scope>
                             <info>
                               <category>Met</category>
                               <event>Severe weather warning</event>
                               <urgency>Immediate</urgency>
                               <severity>Severe</severity>
                               <certainty>Observed</certainty>
                               <area>
                                 <areaDesc>County A</areaDesc>
                               </area>
                             </info>
                           </alert>
                           """;

        var serializer = new XmlSerializer(typeof(CapAlertTransportRequest));
        using var reader = new StringReader(xml);

        var request = (CapAlertTransportRequest?)serializer.Deserialize(reader);

        Assert.NotNull(request);
        Assert.Equal("cap-alert-123", request!.Identifier);
        Assert.Equal("alerts@example.com", request.Sender);
        Assert.Equal(AlertStatus.Actual, request.Status);
        Assert.Equal(AlertMessageType.Alert, request.MessageType);
        Assert.Equal(AlertScope.Public, request.Scope);
        Assert.Single(request.Infos);
        Assert.Equal(AlertInfoCategory.Met, request.Infos[0].Categories.Single());
        Assert.Equal("County A", request.Infos[0].Areas.Single().AreaDescription);
    }

    [Fact]
    public void Deserialize_RealCap12Sample_MapsExpectedFields()
    {
        const string xml = """
                           <?xml version="1.0" encoding="UTF-8"?>
                           <alert xmlns="urn:oasis:names:tc:emergency:cap:1.2">
                             <identifier>43b080713727</identifier>
                             <sender>hsas@dhs.gov</sender>
                             <sent>2003-04-02T14:39:01-05:00</sent>
                             <status>Actual</status>
                             <msgType>Alert</msgType>
                             <scope>Public</scope>
                             <info>
                               <category>Security</category>
                               <event>Homeland Security Advisory System Update</event>
                               <urgency>Immediate</urgency>
                               <severity>Severe</severity>
                               <certainty>Likely</certainty>
                               <senderName>U.S. Government, Department of Homeland Security</senderName>
                               <headline>Homeland Security Sets Code ORANGE</headline>
                               <description>The Department of Homeland Security has elevated the Homeland Security Advisory System threat level to ORANGE / High in response to intelligence which may indicate a heightened threat of terrorism.</description>
                               <instruction>A High Condition is declared when there is a high risk of terrorist attacks. In addition to the Protective Measures taken in the previous Threat Conditions, Federal departments and agencies should consider agency-specific Protective Measures in accordance with their existing plans.</instruction>
                               <web>http://www.dhs.gov/dhspublic/display?theme=29</web>
                               <parameter>
                                 <valueName>HSAS</valueName>
                                 <value>ORANGE</value>
                               </parameter>
                               <resource>
                                 <resourceDesc>Image file (GIF)</resourceDesc>
                                 <mimeType>image/gif</mimeType>
                                 <uri>http://www.dhs.gov/dhspublic/getAdvisoryImage</uri>
                               </resource>
                               <area>
                                 <areaDesc>U.S. nationwide and interests worldwide</areaDesc>
                               </area>
                             </info>
                           </alert>
                           """;

        var serializer = new XmlSerializer(typeof(CapAlertTransportRequest));
        using var reader = new StringReader(xml);

        var request = (CapAlertTransportRequest?)serializer.Deserialize(reader);

        Assert.NotNull(request);
        Assert.Equal("43b080713727", request!.Identifier);
        Assert.Equal("hsas@dhs.gov", request.Sender);
        Assert.Equal(AlertStatus.Actual, request.Status);
        Assert.Equal(AlertMessageType.Alert, request.MessageType);
        Assert.Equal(AlertScope.Public, request.Scope);

        var info = Assert.Single(request.Infos);
        Assert.Equal(AlertInfoCategory.Security, info.Categories.Single());
        Assert.Equal("Homeland Security Advisory System Update", info.Event);
        Assert.Equal(AlertUrgency.Immediate, info.Urgency);
        Assert.Equal(AlertSeverity.Severe, info.Severity);
        Assert.Equal(AlertCertainty.Likely, info.Certainty);
        Assert.Equal("U.S. Government, Department of Homeland Security", info.SenderName);
        Assert.Equal("Homeland Security Sets Code ORANGE", info.Headline);
        Assert.Equal("http://www.dhs.gov/dhspublic/display?theme=29", info.Web);

        var parameter = Assert.Single(info.Parameters);
        Assert.Equal("HSAS", parameter.ValueName);
        Assert.Equal("ORANGE", parameter.Value);

        var resource = Assert.Single(info.Resources);
        Assert.Equal("Image file (GIF)", resource.ResourceDescription);
        Assert.Equal("image/gif", resource.MimeType);
        Assert.Equal("http://www.dhs.gov/dhspublic/getAdvisoryImage", resource.Uri);

        var area = Assert.Single(info.Areas);
        Assert.Equal("U.S. nationwide and interests worldwide", area.AreaDescription);
    }

    [Fact]
    public void Deserialize_WeatherWarningCap12Sample_MapsExpectedFields()
    {
        const string xml = """
                           <?xml version="1.0" encoding="UTF-8"?>
                           <alert xmlns="urn:oasis:names:tc:emergency:cap:1.2">
                             <identifier>KSTO1055887203</identifier>
                             <sender>KSTO@NWS.NOAA.GOV</sender>
                             <sent>2003-06-17T14:57:00-07:00</sent>
                             <status>Actual</status>
                             <msgType>Alert</msgType>
                             <scope>Public</scope>
                             <info>
                               <category>Met</category>
                               <event>SEVERE THUNDERSTORM</event>
                               <responseType>Shelter</responseType>
                               <urgency>Immediate</urgency>
                               <severity>Severe</severity>
                               <certainty>Observed</certainty>
                               <eventCode>
                                 <valueName>SAME</valueName>
                                 <value>SVR</value>
                               </eventCode>
                               <expires>2003-06-17T16:00:00-07:00</expires>
                               <senderName>NATIONAL WEATHER SERVICE SACRAMENTO CA</senderName>
                               <headline>SEVERE THUNDERSTORM WARNING</headline>
                               <description>AT 254 PM PDT...NATIONAL WEATHER SERVICE DOPPLER RADAR INDICATED A SEVERE THUNDERSTORM OVER SOUTH CENTRAL ALPINE COUNTY...OR ABOUT 18 MILES SOUTHEAST OF KIRKWOOD...MOVING SOUTHWEST AT 5 MPH. HAIL...INTENSE RAIN AND STRONG DAMAGING WINDS ARE LIKELY WITH THIS STORM.</description>
                               <instruction>TAKE COVER IN A SUBSTANTIAL SHELTER UNTIL THE STORM PASSES.</instruction>
                               <contact>BARUFFALDI/JUSKIE</contact>
                               <area>
                                 <areaDesc>EXTREME NORTH CENTRAL TUOLUMNE COUNTY IN CALIFORNIA, EXTREME NORTHEASTERN CALAVERAS COUNTY IN CALIFORNIA, SOUTHWESTERN ALPINE COUNTY IN CALIFORNIA</areaDesc>
                                 <polygon>38.47,-120.14 38.34,-119.95 38.52,-119.74 38.62,-119.89 38.47,-120.14</polygon>
                                 <geocode>
                                   <valueName>SAME</valueName>
                                   <value>006109</value>
                                 </geocode>
                                 <geocode>
                                   <valueName>SAME</valueName>
                                   <value>006009</value>
                                 </geocode>
                                 <geocode>
                                   <valueName>SAME</valueName>
                                   <value>006003</value>
                                 </geocode>
                               </area>
                             </info>
                           </alert>
                           """;

        var serializer = new XmlSerializer(typeof(CapAlertTransportRequest));
        using var reader = new StringReader(xml);

        var request = (CapAlertTransportRequest?)serializer.Deserialize(reader);

        Assert.NotNull(request);
        Assert.Equal("KSTO1055887203", request!.Identifier);
        Assert.Equal("KSTO@NWS.NOAA.GOV", request.Sender);
        Assert.Equal(AlertStatus.Actual, request.Status);
        Assert.Equal(AlertMessageType.Alert, request.MessageType);
        Assert.Equal(AlertScope.Public, request.Scope);

        var info = Assert.Single(request.Infos);
        Assert.Equal(AlertInfoCategory.Met, info.Categories.Single());
        Assert.Equal(AlertResponseType.Shelter, info.ResponseTypes.Single());
        Assert.Equal(AlertUrgency.Immediate, info.Urgency);
        Assert.Equal(AlertSeverity.Severe, info.Severity);
        Assert.Equal(AlertCertainty.Observed, info.Certainty);
        Assert.Equal("NATIONAL WEATHER SERVICE SACRAMENTO CA", info.SenderName);
        Assert.Equal("SEVERE THUNDERSTORM WARNING", info.Headline);
        Assert.Equal("BARUFFALDI/JUSKIE", info.Contact);

        var eventCode = Assert.Single(info.EventCodes);
        Assert.Equal("SAME", eventCode.ValueName);
        Assert.Equal("SVR", eventCode.Value);

        var area = Assert.Single(info.Areas);
        Assert.Equal(
            "EXTREME NORTH CENTRAL TUOLUMNE COUNTY IN CALIFORNIA, EXTREME NORTHEASTERN CALAVERAS COUNTY IN CALIFORNIA, SOUTHWESTERN ALPINE COUNTY IN CALIFORNIA",
            area.AreaDescription);
        Assert.Equal("38.47,-120.14 38.34,-119.95 38.52,-119.74 38.62,-119.89 38.47,-120.14", area.Polygons.Single());
        Assert.Equal(3, area.GeoCodes.Count);
        Assert.All(area.GeoCodes, g => Assert.Equal("SAME", g.ValueName));
        Assert.Equal(new[] { "006109", "006009", "006003" }, area.GeoCodes.Select(g => g.Value).ToArray());
    }

    [Fact]
    public void Deserialize_EarthquakeUpdateCap12Sample_MapsExpectedFields()
    {
        const string xml = """
                           <?xml version="1.0" encoding="UTF-8"?>
                           <alert xmlns="urn:oasis:names:tc:emergency:cap:1.2">
                             <identifier>TRI13970876.2</identifier>
                             <sender>trinet@caltech.edu</sender>
                             <sent>2003-06-11T20:56:00-07:00</sent>
                             <status>Actual</status>
                             <msgType>Update</msgType>
                             <scope>Public</scope>
                             <references>trinet@caltech.edu,TRI13970876.1,2003-06-11T20:30:00-07:00</references>
                             <info>
                               <category>Geo</category>
                               <event>Earthquake</event>
                               <urgency>Past</urgency>
                               <severity>Minor</severity>
                               <certainty>Observed</certainty>
                               <senderName>Southern California Seismic Network (TriNet) operated by Caltech and USGS</senderName>
                               <headline>EQ 3.4 Imperial County CA</headline>
                               <description>A minor earthquake measuring 3.4 on the Richter scale occurred near Brawley, California at 8:30 PM Pacific Daylight Time on Wednesday, June 11, 2003. (This event has now been reviewed by a seismologist)</description>
                               <web>http://www.trinet.org/scsn/scsn.html</web>
                               <parameter>
                                 <valueName>EventID</valueName>
                                 <value>13970876</value>
                               </parameter>
                               <parameter>
                                 <valueName>Version</valueName>
                                 <value>1</value>
                               </parameter>
                               <parameter>
                                 <valueName>Magnitude</valueName>
                                 <value>3.4 Ml</value>
                               </parameter>
                               <parameter>
                                 <valueName>Depth</valueName>
                                 <value>11.8 mi.</value>
                               </parameter>
                               <parameter>
                                 <valueName>Quality</valueName>
                                 <value>Excellent</value>
                               </parameter>
                               <area>
                                 <areaDesc>1 mi. WSW of Brawley, CA; 11 mi. N of El Centro, CA; 30 mi. E of OCOTILLO (quarry); 1 mi. N of the Imperial Fault</areaDesc>
                                 <circle>32.9525,-115.5527 0</circle>
                               </area>
                             </info>
                           </alert>
                           """;

        var serializer = new XmlSerializer(typeof(CapAlertTransportRequest));
        using var reader = new StringReader(xml);

        var request = (CapAlertTransportRequest?)serializer.Deserialize(reader);

        Assert.NotNull(request);
        Assert.Equal("TRI13970876.2", request!.Identifier);
        Assert.Equal("trinet@caltech.edu", request.Sender);
        Assert.Equal(AlertMessageType.Update, request.MessageType);
        Assert.Equal("trinet@caltech.edu,TRI13970876.1,2003-06-11T20:30:00-07:00", request.References);

        var info = Assert.Single(request.Infos);
        Assert.Equal(AlertInfoCategory.Geo, info.Categories.Single());
        Assert.Equal(AlertUrgency.Past, info.Urgency);
        Assert.Equal(AlertSeverity.Minor, info.Severity);
        Assert.Equal(AlertCertainty.Observed, info.Certainty);

        Assert.Equal(5, info.Parameters.Count);
        Assert.Equal("EventID", info.Parameters[0].ValueName);
        Assert.Equal("13970876", info.Parameters[0].Value);
        Assert.Equal("Quality", info.Parameters[4].ValueName);
        Assert.Equal("Excellent", info.Parameters[4].Value);

        var area = Assert.Single(info.Areas);
        Assert.Equal(
            "1 mi. WSW of Brawley, CA; 11 mi. N of El Centro, CA; 30 mi. E of OCOTILLO (quarry); 1 mi. N of the Imperial Fault",
            area.AreaDescription);
        Assert.Equal("32.9525,-115.5527 0", area.Circles.Single());
    }
}
