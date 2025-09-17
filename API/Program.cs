using DataService.Application.Mapping;
using DataService.Application.Services;
using DataService.Infrastructure;
using DataService.Infrastructure.Factories;
using DataService.Infrastructure.StorageProviders;
using Infrastructure.StorageProviders.Caching;
using Infrastructure.StorageProviders.FileStorage;
using Infrastructure.StorageProviders.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

/// <summary> 
/// Entry point for the DataService Web API application.
/// Configures multi-tier storage, JWT authentication, authorization policies, CORS, and Swagger documentation.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// Database Configuration
/// <summary>
/// Configures Entity Framework Core with SQL Server for database storage provider.
/// Uses connection string from configuration to establish database connectivity.
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));

// Storage Provider Registration
/// <summary>
/// Registers storage providers with appropriate lifetimes:
/// - CacheStorageProvider as Singleton for in-memory caching
/// - FileStorageProvider as Singleton for file-based storage
/// - DatabaseStorageProvider as Scoped for database operations
/// </summary>
builder.Services.AddScoped<IStorageProvider, CacheStorageProvider>();
builder.Services.AddScoped<IStorageProvider, FileStorageProvider>();
builder.Services.AddScoped<IStorageProvider, DatabaseStorageProvider>();

// Service Registration
/// <summary>
/// Registers core application services:
/// - StorageProviderFactory for creating appropriate storage providers
/// - DataService for business logic operations
/// </summary>
builder.Services.AddScoped<IStorageProviderFactory, StorageProviderFactory>();
builder.Services.AddScoped<IDataService, DataService.Application.Services.DataService>();

// Web API Configuration
/// <summary>
/// Configures standard Web API services including controllers and API exploration.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
/// <summary>
/// Configures Swagger/OpenAPI documentation with JWT Bearer authentication support.
/// Includes security definitions and requirements for protected endpoints.
/// </summary>
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataService API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// AutoMapper Configuration
/// <summary>
/// Registers AutoMapper with the MappingProfile for entity-to-DTO conversions.
/// </summary>
builder.Services.AddAutoMapper(typeof(MappingProfile));

// CORS Configuration
/// <summary>
/// Configures Cross-Origin Resource Sharing (CORS) policy to allow requests from:
/// - Local development environment (http://localhost:3000)
/// - Production domain (https://dataservice.com)
/// Allows all headers and HTTP methods for specified origins.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://dataservice.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Authentication Configuration
/// <summary>
/// Configures JWT Bearer token authentication with validation parameters:
/// - Validates issuer, audience, lifetime, and signing key
/// - Uses symmetric security key for token validation
/// </summary>
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "DataServiceApi",
            ValidAudience = "DataServiceApi",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EGsupersecretkeyofAuthentication"))
        };
    });

// Authorization Configuration
/// <summary>
/// Configures authorization policies based on user roles:
/// - "Admin" policy requires "Admin" role
/// - "User" policy requires "User" role
/// These policies can be applied to controllers or actions for role-based access control.
/// </summary>
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});

/// <summary>
/// Builds the configured web application instance.
/// </summary>
var app = builder.Build();

// Development Environment Configuration
/// <summary>
/// Enables Swagger UI and documentation generation in development environment only.
/// Provides interactive API documentation for testing and exploration.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Database Initialization
/// <summary>
/// Ensures the database is created on application startup using Entity Framework.
/// Creates database schema if it doesn't exist, useful for development and initial deployments.
/// </summary>
/// <remarks>
/// In production environments, consider using proper database migration strategies instead of EnsureCreated().
/// </remarks>
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Middleware Pipeline Configuration
/// <summary>
/// Configures the HTTP request pipeline with middleware in the following order:
/// 1. HTTPS redirection for secure communication
/// 2. CORS policy application for cross-origin requests
/// 3. Authentication middleware for JWT token processing
/// 4. Authorization middleware for policy enforcement
/// 5. Controller routing for API endpoints
/// </summary>
/// <remarks>
/// Note: HTTPS is enabled. For local testing without trusting dev certificate, HTTP can be used.
/// Middleware order is critical - authentication must come before authorization.
/// </remarks>
app.UseHttpsRedirection();

app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

/// <summary>
/// Starts the web application and begins listening for HTTP requests.
/// </summary>
app.Run();
