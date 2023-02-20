using dinhgallery_api.BusinessObjects;
using dinhgallery_api.BusinessObjects.Constants;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Infrastructures.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n"
        + "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n"
        + "Example: 'Bearer access_token'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
          (new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            }),
            new List<string>()
        }
    });
});

services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AppRole.Admin)
        .Build();
});
services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
services.ConfigureOptions(builder.Configuration);
services.AddAppServices();
const string AllowedOrigins = "AllowedOrigins";
services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins, policy =>
    {
        string[] allowedOrigins = builder.Configuration["AllowedOrigins"]
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
services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
});
services.AddScoped<IGalleryFolderRepository, GalleryFolderRepository>();
services.AddScoped<IGalleryFileRepository, GalleryFileRepository>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();
app.UseCors(AllowedOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
