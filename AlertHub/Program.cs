using AlertHub.Application.Alerts.Ingestion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IngestAlertService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
