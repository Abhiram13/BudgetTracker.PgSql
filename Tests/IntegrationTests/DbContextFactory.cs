using BudgetTracker.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

public static class TestDbContextFactory
{
    public static string? GetConnectionString()
    {
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(path: "./appsettings.test.json", optional: false)
            .Build();

        string? connectionString = configuration.GetConnectionString("TestDb");

        return connectionString;
    }

    public static WriteDbContext Create()
    {
        string? connectionString = GetConnectionString();
        DbContextOptions<WriteDbContext>? options = new DbContextOptionsBuilder<WriteDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        WriteDbContext context = new WriteDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}

public abstract class TestBase : IDisposable
{
    protected readonly WriteDbContext _dbContext;
    
    protected TestBase()
    {
        _dbContext = TestDbContextFactory.Create();
        _dbContext.Database.BeginTransaction();
    }

    public void Dispose()
    {
        _dbContext.Database.RollbackTransaction();
        _dbContext.Dispose();
    }
}