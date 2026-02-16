using AlertHub.Application.Alerts;
using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Application.Alerts.Query;
using AlertHub.Application.Common;
using AlertHub.Application.Subscriptions;
using AlertHub.Infrastructure.Alerts.Ingestion;
using AlertHub.Infrastructure.Persistence;
using AlertHub.Infrastructure.Persistence.Subscriptions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.MapControllers();

app.Run();

public partial class Program { }
