using System.Text;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BudgetTracker.Shared.Configurations;

/// <summary>
/// Holds JWT secrets or configs
/// </summary>
public record JwtConfiguration
{
    /// <summary>
    /// Audience of the receiver
    /// </summary>
    [ConfigurationKeyName("AUDIENCE")]
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Issuer of the caller
    /// </summary>
    [ConfigurationKeyName("ISSUER")]
    public string Issuer { get; init; } = string.Empty;
    
    /// <summary>
    /// Secret signing key
    /// </summary>
    [ConfigurationKeyName("SIGNING_KEY")]
    public string SigningKey { get; init; } = string.Empty;
}

/// <summary>
/// Sets JWT configuration by extending <see cref="IConfigureNamedOptions{TOptions}"/>
/// </summary>
/// <remarks>
/// This sets <see cref="TokenValidationParameters"/> with below options 
/// <list type="bullet">
/// <item><description>ValidateIssuer = True</description></item>
/// <item><description>ValidateAudience = True</description></item>
/// <item><description>ValidAudience = (Fetches Audience from secret)</description></item>
/// <item><description>ValidIssuer = (Fetches Issuer from secret)</description></item>
/// <item><description>ValidateLifetime = True</description></item>
/// </list>
/// </remarks>
public class ConfigureJwtOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtConfiguration _config;
    
    /// <inheritdoc cref="ConfigureJwtOptions" />
    /// <param name="config">Config that holds <see cref="JwtConfiguration"/></param>
    public ConfigureJwtOptions(IOptions<JwtConfiguration> config)
    {
        _config = config.Value;
    }
    
    /// <summary>
    /// Sets the <see cref="JwtBearerOptions"/> from <see cref="JwtConfiguration"/>
    /// </summary>
    /// <param name="options"><see cref="JwtBearerOptions"/></param>
    public void Configure(JwtBearerOptions options)
    {
        Configure(Options.DefaultName, options);
    }

    /// <param name="name">JWT Schema name</param>
    /// <param name="options"><see cref="JwtBearerOptions"/></param>
    /// <inheritdoc cref="Configure(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions)" />
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme) return;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _config.Audience,
            ValidIssuer = _config.Issuer,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SigningKey)),
        };
    }
}