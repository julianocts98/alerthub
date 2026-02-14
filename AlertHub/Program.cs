using AlertHub.Application.Alerts.Ingestion;
using AlertHub.Infrastructure.Alerts.Ingestion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<AlertDomainMappingService>();
builder.Services.AddScoped<IngestAlertOrchestrationService>();
builder.Services.AddScoped<ICapAlertParser, JsonCapAlertParser>();
builder.Services.AddScoped<ICapAlertParser, XmlCapAlertParser>();
builder.Services.AddSingleton<ICapXmlSchemaValidator, CapXmlSchemaValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
