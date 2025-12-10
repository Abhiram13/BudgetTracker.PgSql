using Microsoft.EntityFrameworkCore;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Repository;
using BudgetTracker.Finance.Services;

namespace BudgetTracker.Finance.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddCollections(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAuthentication();
        serviceCollection.AddAuthorization();
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
        serviceCollection.AddControllers();
        AddDbContext(serviceCollection);
        AddScopedServices(serviceCollection);

        return serviceCollection;
    }

    private static void AddDbContext(IServiceCollection collection)
    {
        collection.AddDbContext<WriteDbContext>(async (provider, options) =>
        {
            string? postgresHost = "localhost";
            string? postgresPort = "5432";
            string? postgresDatabase = "BudgetTracker.Finance";
            string? postgresUsername = "postgres";
            string? postgresPassword = "postgres";
            string connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
            options.UseNpgsql(connectionString);
        });

        collection.AddDbContext<ReadDbContext>(async (provider, options) =>
        {
            string? postgresHost = "localhost";
            string? postgresPort = "5433";
            string? postgresDatabase = "BudgetTracker.Finance";
            string? postgresUsername = "postgres";
            string? postgresPassword = "postgres";
            string connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
            options.UseNpgsql(connectionString);            
        });
    }

    private static void AddScopedServices(IServiceCollection collection)
    {
        collection.AddScoped<IBankRepository, BankRepository>();
        collection.AddScoped<BankService>();
    }
}