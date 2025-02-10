using Bogus;
using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.Persistence;

namespace SoundexDifferenceDemo.WebApi;

public class QuoteDto
{
    public string Author { get; set; } = default!;
    public List<string> Quotes { get; set; } = default!;
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddPresentation()
            .AddInfrastructure(builder.Configuration)
            .AddApplication()
            .AddPersistence();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            using var identityDatabase = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("InTesting")))
            {
                await identityDatabase.Database.MigrateAsync();
            }

            if (app.Environment.IsDevelopment() && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("InTesting"))
                && !await identityDatabase.Quotes.AnyAsync())
            {
                var faker = new Faker();
                var quoteFaker = new Faker<QuoteDto>()
                    .RuleFor(q => q.Author, f => f.Name.FullName())
                    .RuleFor(q => q.Quotes, f => Enumerable.Range(1, f.Random.Int(1, 99))
                    .Select(_ => f.Lorem.Sentence())
                    .ToList());

                var fakeQuotes = quoteFaker.Generate(1000);

                var quotes = fakeQuotes.SelectMany(q => q.Quotes.Select(quote => new Quote
                {
                    Author = q.Author,
                    Text = quote,
                    NormalizedAuthor = q.Author.Normalize().ToLowerInvariant(),
                    CreatedAt = DateTime.UtcNow
                    .AddDays(-faker.Random.Int(1, 730))
                    .AddHours(-faker.Random.Int(1, 24))
                    .AddMinutes(-faker.Random.Int(1, 60))
                    .AddSeconds(-faker.Random.Int(1, 60))
                })).DistinctBy(q => q.Text);

                await identityDatabase.Quotes.AddRangeAsync(quotes);
                await identityDatabase.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception has occurred while migrating the database.");
            throw;
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler();
        }

        app.Run();
    }
}
