using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Features.Auth;
using Api.Features.Shared;
using Api.Interfaces.Utils;
using Api.Models.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class JwtGenerator(IOptions<JwtOptions> options) : IGenerateJWT
{
    private readonly JwtOptions _options = options.Value;

    public string Generate(string username, EmpoloyeeRole empoloyeeRole)
    {
        Claim[] claims =
        [
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, empoloyeeRole.ToString()),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(_options.ExpiresInHours),
            claims: claims,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
