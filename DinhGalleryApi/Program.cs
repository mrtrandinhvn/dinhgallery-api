using dinhgallery_api.BusinessObjects;
using dinhgallery_api.BusinessObjects.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

const string schemeId = "bearer";
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme.\r\n\r\n"
        + "Enter 'bearer' [space] and then your token in the text input below.\r\n\r\n"
        + "Example: 'bearer access_token'",
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

services
    .AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireRole(AppRole.Admin)
    .Build());

services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
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
            .WithHeaders("Authorization");
    });
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

app.MapGet("/version", () => "2.2.2").AllowAnonymous();

// Log after the application has started
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("dinhgallery_api");
app.Lifetime.ApplicationStarted.Register(() =>
{
    logger.LogInformation("Application started. Environment: {Environment}; ContentRoot: {ContentRootPath}",
        app.Environment.EnvironmentName, builder.Environment.ContentRootPath);
});

app.Run();
