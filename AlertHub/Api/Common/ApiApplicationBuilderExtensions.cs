namespace AlertHub.Api.Common;

public static class ApiApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
