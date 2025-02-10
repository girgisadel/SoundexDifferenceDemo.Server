using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoundexDifferenceDemo.Infrastructure.Contexts;

namespace SoundexDifferenceDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
        .AddDatabase(configuration);

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(DatabaseContext));
        services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));
        return services;
    }
}
