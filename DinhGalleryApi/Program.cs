using System.Threading.RateLimiting;
using dinhgallery_api.BusinessObjects;
using dinhgallery_api.BusinessObjects.Constants;
using dinhgallery_api.Infrastructures.Authentication;
using dinhgallery_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();

// Only configure Swagger in Development
if (builder.Environment.IsDevelopment())
{
    services.AddEndpointsApiExplorer();

    const string schemeId = "bearer";
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

        options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
        {
            Description = "DEVELOPMENT MODE: Authentication is mocked. All requests are automatically authenticated as Admin user. No token required.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "bearer",
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemeId, document)] = []
        });
    });
}

services
    .AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireRole(AppRole.Admin)
    .Build());

services.AddEnvironmentBasedAuthentication(builder.Configuration, builder.Environment);
services.ConfigureOptions(builder.Configuration);
services.ConfigureAppServices(builder.Configuration);
services.ConfigureHostedServices();
const string AllowedOrigins = "AllowedOrigins";
services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins, policy =>
    {
        string[] allowedOrigins = builder.Configuration["AllowedOrigins"]!
            .Split(';')
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Trim())
            .ToArray();
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

const string VersionRateLimitPolicy = "version";
services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter(VersionRateLimitPolicy, opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(5);
        opt.SegmentsPerWindow = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
}

app.UseCors(AllowedOrigins);
app.UseRateLimiter();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    // /storage is where we store all the uploaded files
    RequestPath = "/storage",
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "storage")),
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/version", () => VersionReader.GetCurrentVersion(builder.Environment.ContentRootPath))
    .AllowAnonymous()
    .RequireRateLimiting(VersionRateLimitPolicy);

// Log after the application has started
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("dinhgallery_api");
app.Lifetime.ApplicationStarted.Register(() =>
{
    logger.LogInformation("Application started. Environment: {Environment}; ContentRoot: {ContentRootPath}",
        app.Environment.EnvironmentName, builder.Environment.ContentRootPath);
});

app.Run();
