using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Constants;
using Microsoft.IdentityModel.Tokens;

namespace BudgetTracker.Shared.Security;

public static class JwtFactory
{
    public static string CreateToken(JwtConfiguration configuration, string scope)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.SigningKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Claim[] claims = new Claim[] { new Claim("scope", scope) };
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: configuration.Issuer,
            audience: configuration.Audience,
            claims: claims, 
            expires: DateTime.UtcNow.AddMinutes(1),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}