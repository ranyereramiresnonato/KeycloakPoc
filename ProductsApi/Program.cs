using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PurchasesApi.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "http://keycloak:8080/realms/FintechRealm";
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
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
        OnTokenValidated = async context =>
        {
            var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return;

            // 1️⃣ Pegar o "sub" do token (ID do Keycloak)
            var sub = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? context.Principal?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(sub))
                return;

            // 2️⃣ Obter service via DI
            var keycloakService = context.HttpContext.RequestServices.GetRequiredService<IKeycloakService>();

            // 3️⃣ Buscar roles composite no Keycloak
            var rolesJson = await keycloakService.GetUserCompositeRolesAsync(sub);

            var doc = System.Text.Json.JsonDocument.Parse(rolesJson);

            // 4️⃣ Percorrer o JSON e adicionar roles como Claims
            foreach (var role in doc.RootElement.EnumerateArray())
            {
                if (role.TryGetProperty("name", out var roleName))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName.GetString()!));
                }
            }
        }
    };
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
