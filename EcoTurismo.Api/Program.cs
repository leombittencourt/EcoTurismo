using EcoTurismo.Api.Authorization;
using EcoTurismo.Api.BackgroundServices;
using EcoTurismo.Api.Middleware;
using EcoTurismo.Application.Auth;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Application.Services;
using EcoTurismo.Application.Services.Interfaces;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowFront", policy =>
        policy
            .WithOrigins(
                "https://ecoturismo.lmb.software",
                "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
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

// â”€â”€ Authorization â”€â”€
builder.Services.AddAuthorization(options =>
{
    // Adicionar polÃ­ticas baseadas em roles (mais simples)
    options.AddRolePolicies();

    // Manter polÃ­ticas baseadas em permissions (se necessÃ¡rio)
    options.AddPolicies();
});

// Handlers de autorizaÃ§Ã£o
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// â”€â”€ Memory Cache â”€â”€
builder.Services.AddMemoryCache();

// â”€â”€ Services (DI) â”€â”€
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IQuiosqueService, QuiosqueService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IOcupacaoService, OcupacaoService>();

// â”€â”€ Rate Limiting â”€â”€
builder.Services.AddSingleton(sp => new EcoTurismo.Api.Services.RateLimitingService(
    window: TimeSpan.FromMinutes(1),
    maxRequests: 10
));

// â”€â”€ Geocoding Service â”€â”€
builder.Services.AddHttpClient<IGeocodingService, GoogleMapsGeocodingService>();

// â”€â”€ Image Storage â”€â”€
builder.Services.AddSingleton<EcoTurismo.Application.Services.StorageProviderFactory>();
builder.Services.AddScoped<EcoTurismo.Application.Interfaces.IStorageProvider>(sp =>
{
    var factory = sp.GetRequiredService<EcoTurismo.Application.Services.StorageProviderFactory>();
    return factory.Create();
});
builder.Services.AddScoped<IImageService, EcoTurismo.Application.Services.ImageService>();

// â”€â”€ Background Jobs â”€â”€
builder.Services.AddHostedService<ReconciliacaoOcupacaoJob>();

// â”€â”€ FastEndpoints â”€â”€
builder.Services.AddFastEndpoints();

// Configurar JsonSerializerOptions para FastEndpoints e APIs mÃ­nimas
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

// â”€â”€ InicializaÃ§Ã£o do Banco de Dados â”€â”€
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Iniciando configuraÃ§Ã£o do banco de dados...");

        var db = services.GetRequiredService<EcoTurismoDbContext>();

        // Verificar conexÃ£o
        logger.LogInformation("Testando conexÃ£o com o banco de dados...");
        var canConnect = await db.Database.CanConnectAsync();
        logger.LogInformation("ConexÃ£o com banco: {Status}", canConnect ? "âœ… OK" : "âŒ FALHOU");

        if (!canConnect)
        {
            logger.LogError("NÃ£o foi possÃ­vel conectar ao banco de dados. Verifique a connection string.");
            throw new InvalidOperationException("Falha ao conectar no banco de dados");
        }

        // Verificar migrations pendentes
        var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
        var appliedMigrations = await db.Database.GetAppliedMigrationsAsync();

        logger.LogInformation("Migrations aplicadas: {Count}", appliedMigrations.Count());
        logger.LogInformation("Migrations pendentes: {Count}", pendingMigrations.Count());

        if (pendingMigrations.Any())
        {
            logger.LogInformation("ðŸ“‹ Migrations pendentes:");
            foreach (var migration in pendingMigrations)
            {
                logger.LogInformation("  - {Migration}", migration);
            }
        }

        // Executar migrations automaticamente
        logger.LogInformation("Aplicando migrations pendentes...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Migrations aplicadas com sucesso!");

        // Verificar novamente
        var remainingPending = await db.Database.GetPendingMigrationsAsync();
        if (remainingPending.Any())
        {
            logger.LogWarning("âš ï¸ Ainda hÃ¡ {Count} migrations pendentes apÃ³s MigrateAsync", remainingPending.Count());
        }
        else
        {
            logger.LogInformation("âœ… Todas as migrations foram aplicadas!");
        }

        // Executar seed de dados iniciais
        logger.LogInformation("Executando seed de dados iniciais...");
        await EcoTurismo.Infra.Data.Seeds.AuthorizationSeed.SeedAsync(db);
        logger.LogInformation("Seed executado com sucesso!");

        logger.LogInformation("âœ… Banco de dados configurado e pronto para uso!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "âŒ Erro ao configurar o banco de dados. Detalhes: {Message}", ex.Message);
        logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
        if (ex.InnerException != null)
        {
            logger.LogError("Inner Exception: {InnerMessage}", ex.InnerException.Message);
        }
        throw; // Re-throw para impedir a inicializaÃ§Ã£o com erro
    }
}

app.UseCors("AllowFront");

// â”€â”€ Rate Limiting (primeira camada de proteÃ§Ã£o) â”€â”€
app.UseRateLimiting();

// â”€â”€ Exception Handling Middleware â”€â”€
app.UseExceptionHandling();

// â”€â”€ API Key Validation (para endpoints pÃºblicos protegidos) â”€â”€
app.UseApiKeyValidation();

// â”€â”€ Middleware â”€â”€
app.UseAuthentication();
app.UseAuthorization();

// Configurar FastEndpoints com serializaÃ§Ã£o segura
app.UseFastEndpoints(config =>
{
    config.Serializer.Options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    config.Serializer.Options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    config.Serializer.Options.MaxDepth = 32;
});

// Use Swagger
app.UseSwaggerGen();

app.Run();
