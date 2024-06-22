using APPLICATION.Mapper;
using APPLICATION.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace APPLICATION;
public class AppInjector
{
    public static void Inject(IServiceCollection services, ConfigurationManager configuration)
    {
        // AutoMaper
        services.AddAutoMapper(
           typeof(UserMapper)
           // typeof(/*MapperProfile*/)
        );

        // Jwt
        var cfg = new JwtTokenConfig(
            configuration["Jwt:SecretKey"],
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"]
        );

        services.AddSingleton(cfg);
        services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
        services.AddHostedService<JwtRefreshTokenCache>();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.RequireHttpsMetadata = true;
            option.SaveToken = true;
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(cfg.SecurityKey),
                ValidIssuer = cfg.Issuer,
                ValidAudience = cfg.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(0)
            };
        });
    }
}