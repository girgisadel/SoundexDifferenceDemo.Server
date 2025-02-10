using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SoundexDifferenceDemo.Application.Abstractions.Behaviors;

namespace SoundexDifferenceDemo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly,
            includeInternalTypes: true);

        return services;
    }
}
