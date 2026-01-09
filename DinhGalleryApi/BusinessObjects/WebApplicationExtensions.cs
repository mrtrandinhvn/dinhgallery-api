using dinhgallery_api.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Logging;

namespace dinhgallery_api.BusinessObjects;

public static class WebApplicationExtensions
{
    public static void ConfigureDevelopmentMiddleware(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        app.UseSwagger();
        app.UseSwaggerUI();
        IdentityModelEventSource.ShowPII = true;
    }

    public static void ConfigureMiddlewarePipeline(this WebApplication app)
    {
        app.UseCors(CorsPolicy.AllowedOrigins);
        app.UseRateLimiter();
        app.UseStaticFiles(new StaticFileOptions
        {
            ServeUnknownFileTypes = true,
            RequestPath = "/storage",
            FileProvider = new PhysicalFileProvider(
                Path.Combine(app.Environment.ContentRootPath, "storage")),
        });
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapControllers();

        app.MapGet("/version", () => VersionReader.GetCurrentVersion(app.Environment.ContentRootPath))
            .AllowAnonymous()
            .RequireRateLimiting(RateLimitPolicy.Version);
    }
}
