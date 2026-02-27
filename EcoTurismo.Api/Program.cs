using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Auth;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Application.Services;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EcoTurismoDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsql =>
    {
        npgsql.CommandTimeout(180);
        npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
    });
    o.EnableDetailedErrors();
    o.EnableSensitiveDataLogging();
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ── Authorization ──
builder.Services.AddAuthorization(options =>
{
    options.AddPolicies();
});

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// ── Memory Cache ──
builder.Services.AddMemoryCache();

// ── Services (DI) ──
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IQuiosqueService, QuiosqueService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// ── FastEndpoints ──
builder.Services.AddFastEndpoints();

// Add Swagger for FastEndpoints
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "EcoTurismo API";
        s.Version = "v1";
        s.Description = "API para gerenciamento do turismo local";
    };
});

var app = builder.Build();

// ── Middleware ──
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

// Use Swagger
app.UseSwaggerGen();

app.Run();
