using System.Diagnostics;

namespace AlertHub.Infrastructure.Telemetry;

public static class TelemetryConstants
{
    public const string ServiceName = "AlertHub";
    public static readonly ActivitySource ActivitySource = new(ServiceName);
}
