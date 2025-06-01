using APPLICATION.Jwt;
using APPLICATION.Mapper;
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
        #region AUTOMAPPER
            services.AddAutoMapper(typeof(UserMapper));
			services.AddAutoMapper(typeof(RoleMapper)); /* added by make.py */
			services.AddAutoMapper(typeof(RoleActionMapper)); /* added by make.py */
			services.AddAutoMapper(typeof(UserXAccessMapper)); /* added by make.py */
		#endregion

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