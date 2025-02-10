using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.Infrastructure.Contexts;
using SoundexDifferenceDemo.Persistence.Managers;

namespace SoundexDifferenceDemo.IntegrationTests;

public class BaseTest : IClassFixture<TestWebAppFactory>, IDisposable
{
    private readonly IServiceScope scope;
    protected readonly DatabaseContext dbContext;

    public BaseTest(TestWebAppFactory factory)
    {
        scope = factory.Services.CreateScope();
        dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    }

    public void Dispose()
    {
        dbContext.Dispose();
        scope.Dispose();
    }

    protected async Task ResetDatabaseAsync()
    {
        await dbContext.Quotes.ExecuteDeleteAsync();
    }
}
