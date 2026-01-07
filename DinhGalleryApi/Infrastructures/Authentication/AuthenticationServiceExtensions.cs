using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace dinhgallery_api.Infrastructures.Authentication;

public static class AuthenticationServiceExtensions
{
    /// <summary>
    /// Configures authentication services based on the current environment.
    /// - Development: Uses FakeAuthenticationHandler for automatic authentication
    /// - Production: Uses Azure AD via Microsoft Identity Web
    /// </summary>
    public static IServiceCollection AddEnvironmentBasedAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = FakeAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = FakeAuthenticationHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                FakeAuthenticationHandler.SchemeName,
                options => { });

            // Log warning to make it very clear fake auth is active
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Authentication");

            logger.LogWarning("===== DEVELOPMENT MODE: Using FakeAuthenticationHandler =====");
            logger.LogWarning("All requests will be automatically authenticated as Admin user");
        }
        else
        {
            // Production: Use Azure AD authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration);
        }

        return services;
    }
}
