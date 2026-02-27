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
    // Adicionar políticas baseadas em roles (mais simples)
    options.AddRolePolicies();

    // Manter políticas baseadas em permissions (se necessário)
    options.AddPolicies();
});

// Handlers de autorização
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// ── Memory Cache ──
builder.Services.AddMemoryCache();

// ── Services (DI) ──
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IQuiosqueService, QuiosqueService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// ── FastEndpoints ──
builder.Services.AddFastEndpoints();

// Configurar JsonSerializerOptions para FastEndpoints e APIs mínimas
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.MaxDepth = 32;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

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

// ── Inicialização do Banco de Dados ──
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Iniciando configuração do banco de dados...");

        var db = services.GetRequiredService<EcoTurismoDbContext>();

        // Executar migrations automaticamente
        logger.LogInformation("Aplicando migrations pendentes...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Migrations aplicadas com sucesso!");

        // Executar seed de dados iniciais
        logger.LogInformation("Executando seed de dados iniciais...");
        await EcoTurismo.Infra.Data.Seeds.AuthorizationSeed.SeedAsync(db);
        logger.LogInformation("Seed executado com sucesso!");

        logger.LogInformation("✅ Banco de dados configurado e pronto para uso!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Erro ao configurar o banco de dados. Detalhes: {Message}", ex.Message);
        throw; // Re-throw para impedir a inicialização com erro
    }
}

// ── Middleware ──
app.UseAuthentication();
app.UseAuthorization();

// Configurar FastEndpoints com serialização segura
app.UseFastEndpoints(config =>
{
    config.Serializer.Options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    config.Serializer.Options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    config.Serializer.Options.MaxDepth = 32;
});

// Use Swagger
app.UseSwaggerGen();

app.Run();
