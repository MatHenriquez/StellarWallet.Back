using System.Security.Claims;
using System.Text;
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

internal class Program
{
    private static async Task Main(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";

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

        builder.Services.AddControllers();
        builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("StellarWallet.Infrastructure"))); // Add DatabaseContext
        builder.Services.AddScoped<IUserRepository, UserRepository>(); // Add UserRepository
        builder.Services.AddScoped<IUserService, UserService>(); // Add UserService
        builder.Services.AddScoped<IBlockchainService, StellarService>(); // Add BlockchainService
        builder.Services.AddScoped<ITransactionService, TransactionService>(); // Add TransactionService
        builder.Services.AddScoped<IAuthService, AuthService>(); // Add AuthService
        builder.Services.AddScoped<IJwtService, JwtService>(); // Add JwtService
        builder.Services.AddScoped<IEncryptionService, EncryptionService>(); // Add EncryptionService
        builder.Services.AddScoped<IUserContactRepository, UserContactRepository>(); // Add UserContactRepository
        builder.Services.AddScoped<IUserContactService, UserContactService>(); // Add UserContactService
        builder.Services.AddScoped<IBlockchainAccountRepository, BlockchainAccountRepository>(); // Add BlockchainAccountRepository
        builder.Services.AddAutoMapper(typeof(AutoMapperProfile)); // Add AutoMapper
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddControllers();
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