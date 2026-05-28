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
        /// Loads <see cref="JwtConfiguration"/> with secrets into <see cref="IOptions{TOptions}"/> configuration from <c>Jwt</c> section from <c>appsettings</c>
        /// </summary>
        /// <remarks>This method should be called before <see cref="AddJwtConfiguration"/></remarks>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        /// <returns>Chained <see cref="IServiceCollection"/></returns>
        public IServiceCollection LoadJwtConfiguration(IConfiguration configuration)
        {
            collection
                .AddOptions<JwtConfiguration>()
                .Bind(configuration.GetSection("Jwt"))
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
                .AddJwtBearer();
            
            return collection;
        }
    }
}