using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StellarWallet.Application.Interfaces;
using StellarWallet.Application.Mappers;
using StellarWallet.Application.Services;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Infrastructure;
using StellarWallet.Infrastructure.Repositories;
using StellarWallet.Infrastructure.Services;
using StellarWallet.WebApi.Utilities;

internal class Program
{
    private const string ROUTE_PREFIX = "api";
    private const string DEFAULT_ENVIRONMENT = "test";

    private static async Task Main(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? DEFAULT_ENVIRONMENT;

        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile($"appsettings.{environmentName}.json");

        // Add services to the container.

        // Add authentication services
        builder.Services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(config =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,

                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured."))),
            };
        });

        var connectionString = builder.Configuration.GetConnectionString("StellarWalletDatabase") ?? throw new InvalidOperationException("Undefined connection string");

        // Add roles authorization
        builder.Services.AddAuthorizationBuilder()
                                      .AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "admin"))
                                      .AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "user"));

        // Add DbContext and UnitOfWork
        builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("StellarWallet.Infrastructure")));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add services
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IBlockchainService, StellarService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IEncryptionService, EncryptionService>();
        builder.Services.AddScoped<IUserContactService, UserContactService>();

        // Add AutoMapper
        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

        // Add controllers
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RoutePrefixConvention(ROUTE_PREFIX));
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var corsPolicyName = "AllowClient";

        builder.Services.AddCors(options =>
        {
            var origins = builder.Configuration.GetValue<string>("AllowedHosts") ?? throw new InvalidOperationException("Allowed hosts are not configured.");
            options.AddPolicy(corsPolicyName, builder =>
            {
                builder.WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        app.UseCors(corsPolicyName);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        if (app.Environment.EnvironmentName != "test")
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await db.Database.MigrateAsync();
            }
        }

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}