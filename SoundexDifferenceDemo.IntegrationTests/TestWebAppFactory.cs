using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.WebApi;
using Testcontainers.MsSql;

namespace SoundexDifferenceDemo.IntegrationTests;

public class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("YourStrongPassword123!")
        .Build();

    public async Task InitializeAsync()
    {
        await sqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("InTesting", "TRUE");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
            
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(sqlContainer.GetConnectionString());
            });

            // Apply migrations
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var pendingMigrations = db.Database.GetPendingMigrations()
                .Where(m => m != "20250208204122_AddFullTextIndexOnQuoteText").ToList();

            foreach (var migration in pendingMigrations)
            {
                db.Database.Migrate(migration);
            }
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await sqlContainer.DisposeAsync();
    }
}
