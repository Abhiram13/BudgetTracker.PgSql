using System.Net;
using System.Reflection;
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
using BudgetTracker.Finance.BackgroundWorkers;
using BudgetTracker.Shared.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BudgetTracker.Finance.Extensions;

/// <summary>
/// Holds all extension methods related to <see cref="IServiceCollection"/>
/// </summary>
internal static class ServiceExtension
{
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers all application services, configurations, and third-party integrations into the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This extension method acts as the central method for:
        /// <list type="bullet">
        /// <item><description>Service lifetimes (Scoped, Singleton, Transient)</description></item>
        /// <item><description>Database context</description></item>
        /// <item><description>Options pattern binding and validation</description></item>
        /// <item><description>Background worker services</description></item>
        /// <item><description>API documentation (Swagger) and security schemes</description></item>
        /// </list>
        /// </remarks>
        /// <param name="configuration">The application configuration provided by the host.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
        public IServiceCollection AddCollections(IConfiguration configuration)
        {
            serviceCollection
                .AddLifeTimeServices()
                .AddDbContext()
                .AddOptionsConfigurations(configuration)
                .AddBackgroundServices()
                .AddSwaggerConfiguration()
                .AddControllerConfiguration()
                .AddSecurityConfiguration()
                .AddEndpointsApiExplorer()
                .AddHttpClientConfigurations();
        
            return serviceCollection;
        }

        private IServiceCollection AddBackgroundServices()
        {
            serviceCollection.AddHostedService<OutboxProcessordWorker>();
            serviceCollection.AddHostedService<FinanceHostBackgroundService>();
        
            return serviceCollection;
        }

        private IServiceCollection AddDbContext()
        {
            serviceCollection.AddDbContext<WriteDbContext>((provider, options) =>
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
        
            serviceCollection.AddDbContext<ReadDbContext>((provider, options) =>
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
        
            return serviceCollection;
        }

        private IServiceCollection AddLifeTimeServices()
        {
            serviceCollection.AddScoped<IBankRepository, BankRepository>();
            serviceCollection.AddScoped<ITransactionRepository, TransactionRepository>();
            serviceCollection.AddScoped<ICategoryRepository, CategoryRepository>();
            serviceCollection.AddScoped<ITransactionsMetaRepository, TransactionsMetaRepository>();
            serviceCollection.AddScoped<IOutboxRepository, OutboxRepository>();
            serviceCollection.AddScoped<BankService>();
            serviceCollection.AddScoped<TransactionService>();
            serviceCollection.AddScoped<TransactionsMetaService>();
            serviceCollection.AddScoped<CategoryService>();
            serviceCollection.AddScoped<OutboxService>();
            serviceCollection.AddScoped<TraceIdProvider>();
            serviceCollection.AddScoped<SubscriberService>();
            serviceCollection.AddSingleton<AppSecrets>(sp => sp.GetRequiredService<IOptions<AppSecrets>>().Value);
            serviceCollection.AddSingleton<YarpApiKeySecret>(sp => sp.GetRequiredService<IOptions<AppSecrets>>().Value.Secrets);
            serviceCollection.AddSingleton<PublisherService>();
        
            return serviceCollection;
        }

        private IServiceCollection AddSwaggerConfiguration()
        {
            Action<SwaggerGenOptions> configure = options =>
            {
                string baseDir = AppContext.BaseDirectory;
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(baseDir, xmlFile);
                options.IncludeXmlComments(xmlPath);
    
                string sharedXml = "BudgetTracker.Shared.xml"; 
                string sharedPath = Path.Combine(baseDir, sharedXml);
    
                if (File.Exists(sharedPath))
                {
                    options.IncludeXmlComments(sharedPath);
                }
            
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Budget Tracker Finance API",
                    Version = "v1",
                    Description = "Comprehensive APIs for managing bank transactions and categories."
                });
    
                const string SWAGGER_API_SCHEMA = "Yarp-Api-Key";
                options.AddSecurityDefinition(SWAGGER_API_SCHEMA, new OpenApiSecurityScheme
                {
                    Description = "Yarp api key that gets passed and authenticated to downstream apis",
                    Name = HeaderNames.YARP_API_KEY,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = YarpApiKeySchemaOptions.DefaultSchema,
                });
    
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { 
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = SWAGGER_API_SCHEMA, Type = ReferenceType.SecurityScheme }},
                        Array.Empty<string>()
                    }
                });
            };
        
            serviceCollection.AddSwaggerGen(configure);
        
            return serviceCollection;
        }

        private IServiceCollection AddControllerConfiguration()
        {
            IActionResult ModelValidation(ActionContext action)
            {
                HttpRequest request = action.HttpContext.Request;
                KeyValuePair<string, ModelStateEntry?> modelState = action.ModelState.First(m => m.Value?.Errors.Count > 0);
                string errorAt = modelState.Key;
                string errorMessage = modelState.Value?.Errors.FirstOrDefault()?.ErrorMessage ?? $"Something went wrong at {errorAt}";
                string traceId = request.Headers[HeaderNames.YARP_API_KEY]!;
                ApiResponse<string> apiResponse = new ApiResponse<string> { Message = errorMessage, StatusCode = HttpStatusCode.BadRequest, TraceId = traceId };
                BadRequestObjectResult badRequest = new BadRequestObjectResult(apiResponse);
            
                return badRequest;
            };
        
            serviceCollection.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = ModelValidation;
            });
        
            return serviceCollection;
        }

        private IServiceCollection AddSecurityConfiguration()
        {
            serviceCollection
                .AddAuthentication()
                .AddScheme<YarpApiKeySchemaOptions, YarpApiKeyHandler>(YarpApiKeySchemaOptions.DefaultSchema, _ => {});
        
            serviceCollection.AddAuthorization();
        
            return serviceCollection;
        }

        private IServiceCollection AddHttpClientConfigurations()
        {
            serviceCollection.AddHttpClient<CloudStorageHttpClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:3007/"); // TODO: Get url from appsettings or env vars. Set Auth as well
            });
        
            return serviceCollection;
        }

        private IServiceCollection AddOptionsConfigurations(IConfiguration configuration)
        {
            serviceCollection.AddOptions<AppSecrets>().Bind(configuration).ValidateDataAnnotations().ValidateOnStart();
            
            return serviceCollection;
        }
    }
}