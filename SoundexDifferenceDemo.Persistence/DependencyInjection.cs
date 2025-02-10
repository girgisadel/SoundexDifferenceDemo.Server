using Microsoft.Extensions.DependencyInjection;
using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Application.Abstractions.Repositories;
using SoundexDifferenceDemo.Application.Abstractions.Validators;
using SoundexDifferenceDemo.Persistence.Managers;
using SoundexDifferenceDemo.Persistence.Repositories;
using SoundexDifferenceDemo.Persistence.Validators;

namespace SoundexDifferenceDemo.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
        => services
        .AddRepositories()
        .AddManagers()
        .AddValidators();

    private static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IQuoteManager<>), typeof(QuoteManager<>));
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IQuoteRepository<>), typeof(QuoteRepository<>));
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped(typeof(IQuoteValidator<>), typeof(QuoteValidator<>));
        return services;
    }
}
