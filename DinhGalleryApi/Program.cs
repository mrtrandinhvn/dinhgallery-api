using dinhgallery_api.BusinessObjects;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    services.ConfigureSwagger();
}

services.ConfigureAuthorization();
services.ConfigureAuthentication(builder.Configuration, builder.Environment);
services.ConfigureOptions(builder.Configuration);
services.ConfigureAppServices(builder.Configuration);
services.ConfigureHostedServices();
services.ConfigureCors(builder.Configuration);
services.ConfigureRateLimiting();

WebApplication app = builder.Build();

app.ConfigureDevelopmentMiddleware();
app.ConfigureMiddlewarePipeline();
app.MapEndpoints();
app.ConfigureStartupLogging();

app.Run();
