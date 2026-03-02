using AlertHub.Application.Alerts;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Deliveries;
using AlertHub.Application.Identity;
using AlertHub.Application.Subscriptions;
using AlertHub.Domain.Common;
using AlertHub.Domain.Common.Delivery;
using AlertHub.Domain.Common.Security;
using AlertHub.Infrastructure.Alerts.Ingestion;
using AlertHub.Infrastructure.Alerts.Matching;
using AlertHub.Infrastructure.BackgroundJobs;
using AlertHub.Infrastructure.Delivery.Telegram;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Deliveries;
using AlertHub.Infrastructure.Persistence.Interceptors;
using AlertHub.Infrastructure.Persistence.Subscriptions;
using AlertHub.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace AlertHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IIdentityTokenGenerator, JwtIdentityTokenGenerator>();
        services.AddSingleton<IIdentityIssuerKeyProvider, ConfigurationIdentityIssuerKeyProvider>();

        services.AddScoped<AlertSubscriptionMatcher>();
        services.AddScoped<IAlertDeliveryChannel, TelegramDeliveryChannel>();

        services.AddSingleton<AuditingInterceptor>();
        services.AddHostedService<OutboxPublisher>();
        services.AddHostedService<SubscriptionMatcherWorker>();
        services.AddHostedService<AlertDeliveryWorker>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });

        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddScoped<ICapAlertParser, JsonCapAlertParser>();
        services.AddScoped<ICapAlertParser, XmlCapAlertParser>();
        services.AddSingleton<ICapXmlSchemaValidator, CapXmlSchemaValidator>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
