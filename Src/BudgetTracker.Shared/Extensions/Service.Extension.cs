using System.Net;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace BudgetTracker.Shared.Extensions;

/// <summary>
/// </summary>
public static class SharedServiceExtensions
{
    extension(IServiceCollection collection)
    {
        /// <summary>
        /// Loads <see cref="JwtConfiguration"/> with secrets from <c>JWT</c> section
        /// </summary>
        /// <remarks>This method should be called before <see cref="AddJwtConfiguration"/></remarks>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection LoadJwtConfiguration(IConfiguration configuration)
        {
            collection
                .AddOptions<JwtConfiguration>()
                .Bind(configuration.GetSection("JWT"))
                .Validate(c => !string.IsNullOrEmpty(c.SigningKey), "JWT Signing key is required")
                .Validate(c => !string.IsNullOrEmpty(c.Issuer), "JWT Issuer is required")
                .ValidateOnStart();
            
            return collection;
        }
        
        /// <summary>
        /// Configures JWT <see cref="TokenValidationParameters"/> and sets JWT as default authenticate method and adds <c>Bearer</c> as default schema
        /// </summary>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection AddJwtConfiguration()
        {
            collection
                .ConfigureOptions<ConfigureJwtOptions>()
                .AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            ApiResponse response = new ApiResponse
                            {
                                Message = "You are not authorized. Token may be missing or invalid.",
                                StatusCode = HttpStatusCode.Unauthorized,
                            };

                            await context.Response.WriteAsJsonAsync(response);
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            ApiResponse response = new ApiResponse
                            {
                                StatusCode = HttpStatusCode.Forbidden,
                                Message = "Access Denied: You do not have permission to perform this action."
                            };

                            await context.Response.WriteAsJsonAsync(response);
                        }
                    };
                });
            
            return collection;
        }

        /// <summary>
        /// Registers <see cref="DatabaseConfiguration"/> (which implements <see cref="IConfigureNamedOptions{T}"/>) in <see cref="ConfigureOptions{T}"/>.
        /// </summary>
        /// <remarks>In this service, <c>WRITE</c>, <c>READ</c> and <c>MIGRATE</c> secret values will be configured and loaded in <see cref="IOptions{T}"/>.
        /// Register this service in <see cref="IServiceCollection"/> to load Database credentials in <see cref="IConfiguration"/>
        /// </remarks>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection AddDatabaseConfiguration()
        {
            collection.ConfigureOptions<DatabaseConfigurationSetup>();
            
            return collection;
        }

        /// <summary>
        /// Registers DBContext from given <typeparamref name="TContext"/>
        /// </summary>
        /// <param name="dbName">Name of the Database (<c>WRITE</c>, <c>READ</c>, <c>MIGRATE</c>)</param>
        /// <typeparam name="TContext">DBContext class that should get registered. The class will be extension of <see cref="DbContext"/></typeparam>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection AddPostgresDbContext<TContext>(string dbName) where TContext : DbContext
        {
            collection.AddDbContext<TContext>((provider, options) =>
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
            
            return collection;
        }
        
        /// <summary>
        /// Registers DBContext from given <typeparamref name="TContext"/> that can use existing <see cref="NpgsqlConnection"/>
        /// </summary>
        /// <remarks>Gets <see cref="NpgsqlConnection"/> from <see cref="IServiceProvider"/> and internally builds shared connection with it.</remarks>
        /// <typeparam name="TContext">DBContext class that should get registered. The class will be extension of <see cref="DbContext"/></typeparam>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection AddPostgresDbContext<TContext>() where TContext : DbContext
        {
            collection.AddDbContext<TContext>((provider, options) =>
            {
                NpgsqlConnection sharedConnection = provider.GetRequiredService<NpgsqlConnection>();
                options.UseNpgsql(sharedConnection);
            });
            
            return collection;
        }
    }
}