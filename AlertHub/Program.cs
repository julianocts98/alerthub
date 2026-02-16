using AlertHub.Application.Alerts.Matching;
using AlertHub.Application.Common.Delivery;
using AlertHub.Application.Alerts;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Alerts.Query;
using AlertHub.Application.Common;
using AlertHub.Application.Subscriptions;
using AlertHub.Infrastructure.Alerts.Ingestion;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Subscriptions;
using AlertHub.Infrastructure.BackgroundJobs;
using AlertHub.Infrastructure.Delivery.Telegram;
using AlertHub.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using AlertHub.Application.Common.Security;
using AlertHub.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "a_very_long_secret_key_for_development_purposes"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Roles.Producer, policy => policy.RequireRole(Roles.Producer));
    options.AddPolicy(Roles.Consumer, policy => policy.RequireRole(Roles.Consumer));
    options.AddPolicy(Roles.Admin, policy => policy.RequireRole(Roles.Admin));
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
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
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

public partial class Program { }
