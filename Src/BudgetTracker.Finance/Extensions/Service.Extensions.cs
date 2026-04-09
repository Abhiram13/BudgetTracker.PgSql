using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Abhiram.Secrets.Configuration;
using BudgetTracker.Finance.HttpClients;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Repository;
using BudgetTracker.Finance.Services;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Workers;

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
        serviceCollection.AddHostedService<OutboxProcessordWorker>();
        serviceCollection.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = false;
            options.InvalidModelStateResponseFactory = ModelValidation;
        });
        serviceCollection.AddHttpClient<CloudStorageHttpClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:3007/"); // TODO: Get url from appsettings or env vars. Set Auth as well
        });

        IActionResult ModelValidation(ActionContext action)
        {
            HttpRequest request = action.HttpContext.Request;
            KeyValuePair<string, ModelStateEntry?> modelState = action.ModelState.First(m => m.Value?.Errors.Count > 0);
            string errorAt = modelState.Key;
            string errorMessage = modelState.Value?.Errors.FirstOrDefault()?.ErrorMessage ?? $"Something went wrong at {errorAt}";
            string traceId = request.Headers["X-Trace-Id"]!;
            ApiResponse<string> apiResponse = new ApiResponse<string> { Message = errorMessage, StatusCode = HttpStatusCode.BadRequest, TraceId = traceId };
            BadRequestObjectResult badRequest = new BadRequestObjectResult(apiResponse);
            
            return badRequest;
        };

        return serviceCollection;
    }

    private static void AddDbContext(IServiceCollection collection)
    {
        collection.AddDbContext<WriteDbContext>((provider, options) =>
        {
            PostgresSecrets secrets = provider.GetRequiredService<AppSecrets>().Postgres;

            string? postgresHost = secrets.Host;
            string? postgresPort = secrets.WritePort;
            string? postgresDatabase = secrets.Database;
            string? postgresUsername = secrets.WriteUsername;
            string? postgresPassword = secrets.WritePassword;
            string connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
            options.UseNpgsql(connectionString);
        });
        
        collection.AddDbContext<ReadDbContext>((provider, options) =>
        {
            PostgresSecrets secrets = provider.GetRequiredService<AppSecrets>().Postgres;

            string? postgresHost = secrets.Host;
            string? postgresPort = secrets.ReadPort;
            string? postgresDatabase = secrets.Database;
            string? postgresUsername = secrets.ReadUsername;
            string? postgresPassword = secrets.ReadPassword;
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
        collection.AddScoped<IOutboxRepository, OutboxRepository>();
        collection.AddScoped<BankService>();
        collection.AddScoped<TransactionService>();
        collection.AddScoped<TransactionsMetaService>();
        collection.AddScoped<CategoryService>();
        collection.AddScoped<OutboxService>();
        collection.AddScoped<TraceIdProvider>();
        collection.AddSingleton<AppSecrets>(sp => sp.GetRequiredService<IOptions<AppSecrets>>().Value);
        collection.AddSingleton<YarpApiKeySecret>(sp => sp.GetRequiredService<IOptions<AppSecrets>>().Value.Secrets);
        collection.AddSingleton<PublisherService>();
    }
}