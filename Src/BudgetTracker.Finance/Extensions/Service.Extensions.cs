using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Repository;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Repositories;

namespace BudgetTracker.Finance.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddCollections(this IServiceCollection serviceCollection)
    {
        AddScopedServices(serviceCollection);
        AddDbContext(serviceCollection);
        serviceCollection.AddAuthentication().AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});
        serviceCollection.AddAuthorization();
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
        serviceCollection.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = false;
            options.InvalidModelStateResponseFactory = action =>
            {
                // TODO: Get custom API response and trace id
                KeyValuePair<string, ModelStateEntry?> modelState = action.ModelState.FirstOrDefault();
                string errorAt = modelState.Key;
                string errorMessage = modelState.Value?.Errors?[0].ErrorMessage ?? $"Something went wrong at {errorAt}";
                return new BadRequestObjectResult(new ApiResponse<string> { Message = errorMessage, StatusCode = HttpStatusCode.BadRequest });
            };
        });

        return serviceCollection;
    }

    private static void AddDbContext(IServiceCollection collection)
    {
        collection.AddDbContext<WriteDbContext>((provider, options) =>
        {
            PostgresSecrets secrets = provider.GetRequiredService<AppSecrets>().Postgres;

            string? postgresHost = secrets.Host;
            string? postgresPort = secrets.Port;
            string? postgresDatabase = secrets.Database;
            string? postgresUsername = secrets.Username;
            string? postgresPassword = secrets.Password;
            string connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
            options.UseNpgsql(connectionString);
        });
    }

    private static void AddScopedServices(IServiceCollection collection)
    {
        collection.AddScoped<IBankRepository, BankRepository>();
        collection.AddScoped<ITransactionRepository, TransactionRepository>();
        collection.AddScoped<ICategoryRepository, CategoryRepository>();
        collection.AddScoped<ITransactionsMetaRepository, TransactionsMetaRepository>();
        collection.AddScoped<BankService>();
        collection.AddScoped<TransactionService>();
        collection.AddScoped<TransactionsMetaService>();
        collection.AddScoped<CategoryService>();
        collection.AddScoped<TraceIdProvider>();
        collection.AddSingleton<AppSecrets>(sp => sp.GetRequiredService<IOptions<AppSecrets>>().Value);
        collection.AddSingleton<YarpApiKeySecret>(sp => sp.GetRequiredService<IOptions<AppSecrets>>().Value.Secrets);
        collection.AddSingleton<PublisherService>();
    }
}