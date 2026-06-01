using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Google.Cloud.PubSub.V1;
using Abhiram.Secrets.Configuration;
using Abhiram.Secrets.Providers.Exceptions;
using BudgetTracker.Finance.HttpClients;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Repository;
using BudgetTracker.Finance.Services;
using BudgetTracker.Finance.Configurations;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.BackgroundWorkers;
using BudgetTracker.Shared.Utilities;
using BudgetTracker.Shared.Security;
using BudgetTracker.Shared.Models;
using BudgetTracker.Shared.Interfaces;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Extensions;
using BudgetTracker.Shared.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerGen;
using Encoding = System.Text.Encoding;

[assembly: InternalsVisibleTo("IntegrationTests.Finance")]

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
                .AddDatabaseConfiguration()
                .AddPostgresDbContext<WriteDbContext>(dbName: DatabaseType.WRITE)
                .AddPostgresDbContext<ReadDbContext>(dbName: DatabaseType.READ)
                .AddPostgresDbContext<MigrateDbContext>(dbName: DatabaseType.MIGRATE)
                .AddOptionsConfigurations(configuration)
                .AddBackgroundServices()
                .AddSwaggerConfiguration()
                .AddControllerConfiguration()
                .AddSecurityConfiguration(configuration)
                .AddEndpointsApiExplorer()
                .AddHttpClientConfigurations();
        
            return serviceCollection;
        }

        private IServiceCollection AddBackgroundServices()
        {
            // serviceCollection.AddHostedService<OutboxProcessordWorker>();
            // serviceCollection.AddHostedService<SubscriberBackgroundWorker>();
        
            return serviceCollection;
        }

        private IServiceCollection AddLifeTimeServices()
        {
            serviceCollection.AddScoped<IBankRepository, BankRepository>();
            serviceCollection.AddScoped<ITransactionRepository, TransactionRepository>();
            serviceCollection.AddScoped<ICategoryRepository, CategoryRepository>();
            serviceCollection.AddScoped<ITransactionsMetaRepository, TransactionsMetaRepository>();
            serviceCollection.AddScoped<IOutboxRepository, OutboxRepository>();
            serviceCollection.AddScoped<IDueRepository, DueRepository>();
            serviceCollection.AddScoped<BankService>();
            serviceCollection.AddScoped<TransactionService>();
            serviceCollection.AddScoped<TransactionsMetaService>();
            serviceCollection.AddScoped<CategoryService>();
            serviceCollection.AddScoped<OutboxService>();
            serviceCollection.AddScoped<DueService>();
            serviceCollection.AddScoped<TraceIdProvider>();
            serviceCollection.AddSingleton<AppSecrets>(sp => sp.GetRequiredService<IOptions<AppSecrets>>().Value);
            serviceCollection.AddSingleton<PublisherClient>(provider =>
            {
                AppSecrets secret = provider.GetRequiredService<AppSecrets>();
                string projectId = secret.GoogleCloudProjectId;
                TopicName topicName = TopicName.FromProjectTopic(projectId, secret.PubSub.Topic);

                return PublisherClient.Create(topicName);
            });
            serviceCollection.AddSingleton<SubscriberClient>(provider =>
            {
                AppSecrets secret = provider.GetRequiredService<AppSecrets>();
                string projectId = secret.GoogleCloudProjectId;
                string subscriberName = secret.PubSub.Subscriber;
                SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriberName);
                SubscriberClient subscriberClient = SubscriberClient.Create(subscriptionName);
                
                return subscriberClient;
            });
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
            
                const string SWAGGER_API_SCHEMA = "Bearer";
                options.AddSecurityDefinition(SWAGGER_API_SCHEMA, new OpenApiSecurityScheme
                {
                    Description = "Enter your JWT Token",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
                    BearerFormat = "JWT"
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
                string traceId = request.Headers[SharedConstants.Headers.YARP_API_KEY]!;
                ApiResponse apiResponse = new ApiResponse { Message = errorMessage, StatusCode = HttpStatusCode.BadRequest, TraceId = traceId };
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

        private IServiceCollection AddSecurityConfiguration(IConfiguration configuration)
        {
            serviceCollection
                .LoadJwtConfiguration(configuration)
                .AddJwtConfiguration()
                .AddAuthorization(options =>
                {
                    options
                        .AddPolicy(SharedConstants.Jwt.Policies.DOWNSTREAM_POLICY, policy =>
                        {
                            string? environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT");
                            
                            switch (environment)
                            {
                                case SharedConstants.Environments.GOOGLECLOUD:
                                    policy.RequireAuthenticatedUser();
                                    break;
                                default:
                                    policy.RequireClaim("scope", SharedConstants.Jwt.Scopes.DOWNSTREAM);
                                    break;
                            }
                        });
                });
        
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

        /// <summary>
        /// Sets config and secrets from environmental variables or appsettings.json into <see cref="IOptions{AppSecrets}"/> 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>Updated <see cref="IServiceCollection"/></returns>
        /// <exception cref="OptionsValidationException">
        /// <para>When <c>Port</c> is invalid or empty</para>
        /// <para>When <c>GOOGLE_CLOUD_PROJECT_ID</c> is invalid or empty</para>
        /// </exception>
        private IServiceCollection AddOptionsConfigurations(IConfiguration configuration)
        {
            serviceCollection
                .AddOptions<AppSecrets>()
                .Bind(configuration)
                .ValidateDataAnnotations()
                .Validate(a => !string.IsNullOrEmpty(a.GoogleCloudProjectId), "Google Cloud Project ID is required and current given value is invalid")
                .Validate<IHostEnvironment>((appConfiguration, hostEnvironment) =>
                {
                    if (hostEnvironment.IsEnvironment(SharedConstants.Environments.TEST))
                    {
                        return true;
                    }

                    return appConfiguration.ServerPort != default;
                }, "Server Port is required and current given value is invalid")
                .ValidateOnStart();
            
            return serviceCollection;
        }
        
        /// <summary>
        /// Registers <see cref="DatabaseConfigurationSetup"/> which allows <see cref="DatabaseConfiguration"/> to be accessed through <see cref="IOptionsMonitor{TOptions}"/>
        /// </summary>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection AddDatabaseConfiguration()
        {
            serviceCollection.ConfigureOptions<DatabaseConfigurationSetup>();
            
            return serviceCollection;
        }

        /// <summary>
        /// Registers <typeparamref name="TContext"/> based DBContext by acquiring <see cref="IOptionsMonitor{DatabaseConfiguration}"/> and initializing a new connection.
        /// </summary>
        /// <param name="dbName">Name of the Database (<c>WRITE</c>, <c>READ</c>, <c>MIGRATE</c>)</param>
        /// <typeparam name="TContext">DBContext class that should get registered. The class will be extension of <see cref="DbContext"/></typeparam>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection AddPostgresDbContext<TContext>(string dbName) where TContext : DbContext
        {
            serviceCollection.AddDbContext<TContext>((provider, options) =>
            {
                DatabaseConfiguration dbConfig = provider.GetRequiredService<IOptionsMonitor<DatabaseConfiguration>>().Get(dbName);

                string? postgresHost = dbConfig.Host;
                string? postgresPort = dbConfig.Port;
                string? postgresDatabase = dbConfig.Database;
                string? postgresUsername = dbConfig.Username;
                string? postgresPassword = dbConfig.Password;
                string connectionString = $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUsername};Password={postgresPassword}";
                options.UseNpgsql(connectionString);
            });
            
            return serviceCollection;
        }
        
        /// <summary>
        /// Registers <typeparamref name="TContext"/> based DBContext by acquiring <see cref="NpgsqlConnection"/> and initializing a new connection.
        /// </summary>
        /// <remarks>Gets <see cref="NpgsqlConnection"/> from <see cref="IServiceProvider"/> and internally builds shared connection with it.</remarks>
        /// <typeparam name="TContext">DBContext class that should get registered. The class will be extension of <see cref="DbContext"/></typeparam>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection AddPostgresDbContext<TContext>() where TContext : DbContext
        {
            serviceCollection.AddDbContext<TContext>((provider, options) =>
            {
                NpgsqlConnection sharedConnection = provider.GetRequiredService<NpgsqlConnection>();
                options.UseNpgsql(sharedConnection);
            });
            
            return serviceCollection;
        }
    }
}