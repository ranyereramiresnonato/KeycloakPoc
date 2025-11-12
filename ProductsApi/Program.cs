using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "http://keycloak:8080/realms/FintechRealm";
    options.Audience = "account";
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuers = new[]
        {
            "http://localhost:8081/realms/FintechRealm",
            "http://keycloak:8081/realms/FintechRealm"
        }
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return Task.CompletedTask;

            var realmRoles = context.Principal?.FindFirst("realm_access")?.Value;
            if (!string.IsNullOrEmpty(realmRoles))
            {
                var parsed = System.Text.Json.JsonDocument.Parse(realmRoles);
                if (parsed.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                    }
                }
            }
            return Task.CompletedTask;
        },
    };
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
