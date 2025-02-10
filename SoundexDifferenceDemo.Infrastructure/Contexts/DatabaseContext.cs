using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Infrastructure.Contexts;

public class DatabaseContext(DbContextOptions<DatabaseContext> options, IPublisher publisher) : DbContext(options)
{
    public DbSet<Quote> Quotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        var methodInfo = typeof(InternalDbFunctionsExtensions).GetMethod(nameof(InternalDbFunctionsExtensions.Soundex), [typeof(string)]);
        if (methodInfo != null)
        {
            modelBuilder
                .HasDbFunction(methodInfo)
                .HasTranslation(args => new SqlFunctionExpression("SOUNDEX", args, false, [false], typeof(string), null));
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    public override int SaveChanges()
    {
        int result = base.SaveChanges();

        PublishDomainEventsAsync().Wait();

        return result;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        int result = base.SaveChanges(acceptAllChangesOnSuccess);

        PublishDomainEventsAsync().Wait();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<IEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}
