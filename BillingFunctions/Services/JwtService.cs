using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BillingFunctions.Services;

public class JwtService
{
    public string GenerateToken(string userName)
    {
        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "BillingApplicationSecretKey@2026SuperSecure"));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(
                        ClaimTypes.Name,
                        userName)
                },
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }


    public bool ValidateToken(string token)
{
    try
    {
        var tokenHandler =
            new JwtSecurityTokenHandler();

        var key =
            Encoding.UTF8.GetBytes(
                "BillingApplicationSecretKey@2026SuperSecure");

        tokenHandler.ValidateToken(
            token,
            new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            },
            out SecurityToken validatedToken);

        return true;
    }
    catch
    {
        return false;
    }
}
}