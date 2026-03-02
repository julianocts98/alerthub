using System.Text;
using AlertHub.Domain.Common.Security;
using AlertHub.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AlertHub.Api.Common;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiPresentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers().AddXmlSerializerFormatters();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtIssuer = RequireSetting(configuration, "Jwt:Issuer");
        var jwtAudience = RequireSetting(configuration, "Jwt:Audience");
        var jwtKey = RequireSetting(configuration, "Jwt:Key");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        return services;
    }

    public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Roles.Admin, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Scopes.AlertsIngest, policy => policy.RequireClaim("scope", Scopes.AlertsIngest));
            options.AddPolicy(Roles.Subscriber, policy => policy.RequireRole(Roles.Subscriber));
        });

        return services;
    }

    public static IServiceCollection AddApiObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["OpenTelemetry:ServiceName"];
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            serviceName = "AlertHub";
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource("AlertHub")
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddConsoleExporter());

        return services;
    }

    private static string RequireSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        throw new InvalidOperationException($"Missing required configuration value '{key}'.");
    }
}
