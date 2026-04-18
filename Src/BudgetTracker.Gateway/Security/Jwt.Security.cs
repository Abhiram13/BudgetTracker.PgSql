using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BudgetTracker.Shared.Constants;
using Microsoft.IdentityModel.Tokens;

namespace BudgetTracker.Gateway.Security;

public static class JwtTokenGenerator
{
    public static string CreateToken(string secretKey, string scope, string audience)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Claim[] claims = new Claim[] { new Claim("scope", scope) };
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: SharedConstants.Jwt.Issuer,
            audience: audience,
            claims: claims, 
            expires: DateTime.UtcNow.AddMinutes(1), 
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



