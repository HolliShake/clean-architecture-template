using System.Runtime.InteropServices;
using INFRASTRUCTURE.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
namespace API;

public class ApiInjector
{
    public static void Inject(IServiceCollection services, ConfigurationManager configuration)
    {
        // Sql
        services.AddDbContext<AppDbContext>(option => option.UseSqlServer(
            (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                ? configuration.GetConnectionString("win32")
                : configuration.GetConnectionString("linux")
        ));
        
        // Authentication
        services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Auth:Google:ClientId"];
                options.ClientSecret = configuration["Auth:Google:ClientSecret"];
            });
        
        // Cors
        services.AddCors();

        // Async io
        services.Configure<IISServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });

        // Swagger
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",

                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            
            options.SwaggerDoc("v1", new OpenApiInfo { Title = configuration["App"], Description = "Api description here...", Version = "v1" });
          
            var filePath = Path.Combine(System.AppContext.BaseDirectory, "API.xml");
            options.IncludeXmlComments(filePath);
            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });
            
        });

        // File hosting
        var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(configuration["File:LocationWIN32"], configuration["File:DestinationWIN32"])
            : Path.Combine(configuration["File:LocationLINUX"], configuration["File:DestinationLINUX"]);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(path));
        }
        else
        {
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(path));
        }
    }
}
