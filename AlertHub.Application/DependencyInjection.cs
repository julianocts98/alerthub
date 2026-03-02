using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Alerts.Query;
using AlertHub.Application.Deliveries;
using AlertHub.Application.Identity;
using AlertHub.Application.Subscriptions;

namespace AlertHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AlertFactory>();
        services.AddScoped<IAlertIngestionService, AlertIngestionService>();
        services.AddScoped<IAlertQueryService, AlertQueryService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
