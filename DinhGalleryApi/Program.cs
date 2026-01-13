using dinhgallery_api.BusinessObjects;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
);

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

app.Run();
