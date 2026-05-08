using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BudgetTracker.Shared.Configurations;

/// <summary>
/// Holds JWT secrets like <see cref="Audience"/>, <see cref="Issuer"/>, <see cref="SigningKey"/> and <see cref="ValidateIssuerSigingKey"/>
/// </summary>
public record JwtConfiguration
{
    /// <summary>
    /// Audience meant to receive the token
    /// </summary>
    [ConfigurationKeyName("Audience")]
    public string Audience { get; set; } = string.Empty; // NOTE: This property should be set dynamically in cloud environment. So, 'set;' 
    
    /// <summary>
    /// Who created the token and making the request
    /// </summary>
    [ConfigurationKeyName("Issuer")]
    public string Issuer { get; init; } = string.Empty;
    
    /// <summary>
    /// Secret signing key
    /// </summary>
    [ConfigurationKeyName("Signingkey")]
    public string SigningKey { get; init; } = string.Empty;
    
    /// <summary>
    /// Should issuer signing key be validated? If <c>false</c>, even tokens with un-matched signing key will be passed. 
    /// </summary>
    /// <remarks>This flag is set to <c>false</c> in Google Cloud environment. Google Cloud IAM auth has it's own signing key setup</remarks>
    /// <value>true</value>
    [ConfigurationKeyName("ValidateIssuerSigningKey")]
    public bool ValidateIssuerSigingKey {  get; init; } = true;
    
}

/// <summary>
/// Sets JWT configuration by extending <see cref="IConfigureNamedOptions{TOptions}"/>
/// </summary>
/// <remarks>
/// <para>
/// This sets <see cref="TokenValidationParameters"/> with below options 
/// <list type="bullet">
/// <item><description>ValidateIssuer = True</description></item>
/// <item><description>ValidateAudience = True</description></item>
/// <item><description>ValidAudience = (Fetches Audience from secret)</description></item>
/// <item><description>ValidIssuer = (Fetches Issuer from secret)</description></item>
/// <item><description>ValidateLifetime = True</description></item>
/// </list>
/// </para>
/// <para>
/// Sets events like <c>OnChallenge</c>, <c>OnForbidden</c> with custom <see cref="ApiResponse"/>
/// </para>
/// <para>Sets <see cref="JwtBearerOptions.IncludeErrorDetails"/> to true</para>
/// </remarks>
public class ConfigureJwtOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtConfiguration _config;
    
    /// <inheritdoc cref="ConfigureJwtOptions" />
    /// <param name="config"><see cref="IOptions{T}"/> Config that holds <see cref="JwtConfiguration"/></param>
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
            ValidateIssuerSigningKey = _config.ValidateIssuerSigingKey,
            SignatureValidator = (token, _) => new JsonWebToken(token),
        };
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
    }
}