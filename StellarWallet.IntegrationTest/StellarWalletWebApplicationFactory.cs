using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StellarWallet.Infrastructure;

namespace StellarWallet.IntegrationTest;

internal class StellarWalletWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DatabaseContext>));
            var connectionString = GetConnectionString();

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            var dbContext = CreateDatabaseContext(services);
            dbContext.Database.EnsureCreated();
        });
    }

    private static string? GetConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<StellarWalletWebApplicationFactory>()
            .Build();

        return configuration.GetConnectionString("StellarWalletTestDatabase");
    }

    private static DatabaseContext CreateDatabaseContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    }
}
