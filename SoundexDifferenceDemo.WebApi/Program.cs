using Bogus;
using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.Persistence;
using SoundexDifferenceDemo.WebApi.Models;
using System.Text.Json;

namespace SoundexDifferenceDemo.WebApi;

public class Program
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

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

            await identityDatabase.Database.MigrateAsync();

            if (app.Environment.IsDevelopment() && !await identityDatabase.Quotes.AnyAsync())
            {
                var quotesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "quotes.json");

                if (File.Exists(quotesFilePath))
                {
                    var json = await File.ReadAllTextAsync(quotesFilePath);
                    var quoteSeedItems = JsonSerializer.Deserialize<List<QuoteSeedItem>>(json, JsonSerializerOptions)?.DistinctBy(q => q.Quote);

                    if (quoteSeedItems != null)
                    {
                        var faker = new Faker();
                        
                        var quotes = quoteSeedItems.Select(q => new Quote
                        {
                            Author = q.Author,
                            Text = q.Quote,
                            NormalizedAuthor = q.Author?.Normalize().ToLowerInvariant(),
                            CreatedAt = DateTime.UtcNow
                            .AddDays(-faker.Random.Int(1, 730))
                            .AddHours(-faker.Random.Int(1, 48))
                            .AddMinutes(-faker.Random.Int(1, 120))
                            .AddSeconds(-faker.Random.Int(1, 120))
                        });

                        await identityDatabase.Quotes.AddRangeAsync(quotes);
                        await identityDatabase.SaveChangesAsync();
                    }
                }
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
