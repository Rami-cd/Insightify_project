using Auth_service.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Auth Service Running");

app.MapPost("/Register", () => "");

app.MapPost("/Login", () => "");

app.MapGet("/me", (ClaimsPrincipal user) =>
{
    var userId = user.FindFirst("sub")?.Value;
    var tenantId = user.FindFirst("tenant")?.Value;

    return new { userId, tenantId };
}).RequireAuthorization();

app.MapGet("/jwt", () =>
{
    var keyBytes = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(keyBytes);
    }

    string base64Secret = Convert.ToBase64String(keyBytes);

    string urlSafeSecret = base64Secret.TrimEnd('=').Replace('+', '-').Replace('/', '_');

    return Results.Ok(urlSafeSecret);
});

app.Run();