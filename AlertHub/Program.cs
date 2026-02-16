using System.Text;
using AlertHub.Application.Alerts;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Alerts.Query;
using AlertHub.Application.Common;
using AlertHub.Application.Common.Delivery;
using AlertHub.Application.Common.Security;
using AlertHub.Application.Deliveries;
using AlertHub.Application.Subscriptions;
using AlertHub.Infrastructure.Alerts.Ingestion;
using AlertHub.Infrastructure.Alerts.Matching;
using AlertHub.Infrastructure.BackgroundJobs;
using AlertHub.Infrastructure.Delivery.Telegram;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Deliveries;
using AlertHub.Infrastructure.Persistence.Interceptors;
using AlertHub.Infrastructure.Persistence.Subscriptions;
using AlertHub.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = RequireSetting(builder.Configuration, "Jwt:Issuer");
var jwtAudience = RequireSetting(builder.Configuration, "Jwt:Audience");
var jwtKey = RequireSetting(builder.Configuration, "Jwt:Key");

// Security Configuration
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Roles.Admin, policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy(Scopes.AlertsIngest, policy => policy.RequireClaim("scope", Scopes.AlertsIngest));
    options.AddPolicy(Roles.Subscriber, policy => policy.RequireRole(Roles.Subscriber));
});

// OpenTelemetry Configuration
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("AlertHub"))
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

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<AlertSubscriptionMatcher>();
builder.Services.AddScoped<IAlertDeliveryChannel, TelegramDeliveryChannel>();

builder.Services.AddSingleton<AuditingInterceptor>();
builder.Services.AddHostedService<OutboxPublisher>();
builder.Services.AddHostedService<SubscriptionMatcherWorker>();
builder.Services.AddHostedService<AlertDeliveryWorker>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
});

builder.Services.AddScoped<AlertFactory>();
builder.Services.AddScoped<AlertIngestionService>();
builder.Services.AddScoped<AlertQueryService>();
builder.Services.AddScoped<DeliveryService>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<ICapAlertParser, JsonCapAlertParser>();
builder.Services.AddScoped<ICapAlertParser, XmlCapAlertParser>();
builder.Services.AddSingleton<ICapXmlSchemaValidator, CapXmlSchemaValidator>();

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static string RequireSetting(IConfiguration configuration, string key)
{
    var value = configuration[key];
    if (!string.IsNullOrWhiteSpace(value))
        return value;

    throw new InvalidOperationException($"Missing required configuration value '{key}'.");
}

public partial class Program { }
