using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AEspejo.Clinic.API.Middleware;
using AEspejo.Clinic.API.Seeding;
using AEspejo.Clinic.API.Services;
using AEspejo.Clinic.Application;
using AEspejo.Clinic.Application.Auth;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Infrastructure;
using AEspejo.Clinic.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var masterConnection = builder.Configuration.GetConnectionString("MasterConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'MasterConnection'.");

// Database engine: SqlServer (default, production) or Sqlite (cheap demo). Selected via Database:Provider.
var dbProvider = DatabaseProviderExtensions.Parse(builder.Configuration["Database:Provider"]);

// Let appsettings pick the SQLite data directory (ResolveDataDirectory reads it from SQLITE_DATA_DIR).
// An already-set env var (e.g. the "sqlite" launch profile) wins.
var sqliteDataDir = builder.Configuration["Database:SqliteDataDirectory"];
if (!string.IsNullOrWhiteSpace(sqliteDataDir)
    && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQLITE_DATA_DIR")))
{
    Environment.SetEnvironmentVariable("SQLITE_DATA_DIR", sqliteDataDir);
}

// ---- Serialization: enums as strings, camelCase ↔ PascalCase mapping ----
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        // Map camelCase JSON properties to PascalCase .NET properties
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ---- Layers ----
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddInfrastructure(masterConnection, dbProvider);
builder.Services.AddApplication();

// ---- JWT authentication ----
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Falta la sección de configuración 'Jwt'.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey))
        };
    });

builder.Services.AddAuthorization();

// ---- CORS (frontend React/Vite) ----
const string CorsPolicy = "SpaCors";
var allowedOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? ["http://localhost:5173"];
builder.Services.AddCors(options =>
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

// ---- OpenAPI + Scalar ----
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Seed on development, or whenever explicitly enabled (e.g. the Azure SQLite demo runs as non-Development).
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Seed:Enabled"))
{
    await DevSeeder.SeedAsync(app.Services, masterConnection);
}

app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
