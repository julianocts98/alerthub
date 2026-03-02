namespace AlertHub.Domain.Alert;

/// <summary>
/// CAP 1.2 <category> code values allowed inside an <info> block.
/// </summary>
public enum AlertInfoCategory
{
    /// <summary>Geo - Geophysical (including landslide).</summary>
    Geo,

    /// <summary>Met - Meteorological (including flood).</summary>
    Met,

    /// <summary>Safety - General emergency and public safety.</summary>
    Safety,

    /// <summary>Security - Law enforcement, military, homeland and local/private security.</summary>
    Security,

    /// <summary>Rescue - Rescue and recovery.</summary>
    Rescue,

    /// <summary>Fire - Fire suppression and rescue.</summary>
    Fire,

    /// <summary>Health - Medical and public health.</summary>
    Health,

    /// <summary>Env - Pollution and other environmental.</summary>
    Env,

    /// <summary>Transport - Public and private transportation.</summary>
    Transport,

    /// <summary>Infra - Utility, telecommunication, other non-transport infrastructure.</summary>
    Infra,

    /// <summary>CBRNE - Chemical, Biological, Radiological, Nuclear or High-Yield Explosive threat or attack.</summary>
    CBRNE,

    /// <summary>Other - Other events.</summary>
    Other
}
