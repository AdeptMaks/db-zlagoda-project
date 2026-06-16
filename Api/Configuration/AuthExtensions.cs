using Api.Models.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Configuration;

public static class AuthExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var cookieToken = context.Request.Cookies["AuthToken"];
                        if (!string.IsNullOrEmpty(cookieToken))
                            context.Token = cookieToken;

                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();
        return services;

    }
}
