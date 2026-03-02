using AlertHub.Api.Common;
using AlertHub.Application;
using AlertHub.Infrastructure;
using AlertHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiPresentation()
    .AddApiAuthentication(builder.Configuration)
    .AddApiAuthorization()
    .AddApiObservability(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseApiPipeline();
app.MapControllers();

app.Run();

public partial class Program { }
