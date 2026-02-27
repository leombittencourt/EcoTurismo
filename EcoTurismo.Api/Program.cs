using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Application.Services;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── EF Core ──
builder.Services.AddDbContext<EcoTurismoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Authentication ──
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
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

var app = builder.Build();

// ── Middleware ──
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();
