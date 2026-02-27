using EcoTurismo.Application.Interfaces;
using EcoTurismo.Application.Services;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddAuthorization();

// ── Services (DI) ──
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IQuiosqueService, QuiosqueService>();

// ── FastEndpoints ──
builder.Services.AddFastEndpoints();

var app = builder.Build();

// ── Middleware ──
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();
