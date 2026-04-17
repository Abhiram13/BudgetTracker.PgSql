using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtConstants = BudgetTracker.Shared.Constants.JwtConstants;

namespace BudgetTracker.Gateway.Security;

public static class JwtTokenGenerator
{
    public static string CreateToken(string secretKey, string scope, string audience)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Claim[] claims = new Claim[1] { new Claim("scope", scope) };
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: JwtConstants.Issuer,
            audience: audience,
            claims: claims, 
            expires: DateTime.UtcNow.AddMinutes(1), 
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



