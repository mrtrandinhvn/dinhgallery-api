using System.Security.Claims;
using System.Text.Encodings.Web;
using dinhgallery_api.BusinessObjects.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace dinhgallery_api.Infrastructures.Authentication;

/// <summary>
/// Fake authentication handler for Development environment only.
/// Automatically authenticates all requests with a mock Admin user.
/// </summary>
public class FakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "FakeScheme";

    public FakeAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Create claims for the fake user
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Development User"),
            new Claim(ClaimTypes.Email, "dev@localhost"),
            new Claim(ClaimTypes.NameIdentifier, "dev-user-id"),
            new Claim(ClaimTypes.Role, AppRole.Admin), // Critical: Admin role
            // Additional claims that Azure AD might provide
            new Claim("preferred_username", "dev@localhost"),
            new Claim("name", "Development User")
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        Logger.LogInformation(
            "[FAKE AUTH] Request authenticated with fake development user. Role: {Role}",
            AppRole.Admin);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
