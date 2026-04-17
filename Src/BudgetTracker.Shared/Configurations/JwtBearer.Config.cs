using System.Text;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BudgetTracker.Shared.Configurations;

public class ConfigureJwtOptions<T> : IConfigureNamedOptions<JwtBearerOptions> where T : SharedSecrets
{
    private readonly JwtSecret _secret;

    public ConfigureJwtOptions(IOptions<T> secret)
    {
        _secret = secret.Value.JwtSecret;
    }
    
    public void Configure(JwtBearerOptions options)
    {
        Configure(Options.DefaultName, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _secret.Audience,
                ValidIssuer = JwtConstants.Issuer,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret.Key)),
            };
        }
    }
}