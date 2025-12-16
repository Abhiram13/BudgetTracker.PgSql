using Microsoft.EntityFrameworkCore;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Repository;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Shared.Security;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Shared.Models;
using System.Net;

namespace BudgetTracker.Finance.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddCollections(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAuthentication().AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});
        serviceCollection.AddAuthorization();
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
        serviceCollection.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = false;
            options.InvalidModelStateResponseFactory = action =>
            {
                KeyValuePair<string, ModelStateEntry?> modelState = action.ModelState.FirstOrDefault();
                string errorAt = modelState.Key;
                string errorMessage = modelState.Value?.Errors?[0].ErrorMessage ?? $"Something went wrong at {errorAt}";
                return new BadRequestObjectResult(new ApiResponse<string> { Message = errorMessage, StatusCode = HttpStatusCode.BadRequest });
            };
        });
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

    }

    private static void AddScopedServices(IServiceCollection collection)
    {
        collection.AddScoped<IBankRepository, BankRepository>();
        collection.AddScoped<ITransactionRepository, TransactionRepository>();
        collection.AddScoped<ICategoryRepository, CategoryRepository>();
        collection.AddScoped<BankService>();
        collection.AddScoped<TransactionService>();
        collection.AddScoped<CategoryService>();
        collection.AddScoped<TraceIdProvider>();
        collection.AddSingleton<PublisherService>();
    }
}