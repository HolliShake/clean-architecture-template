using INFRASTRUCTURE.Data;
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
            configuration.GetConnectionString("win32")
        ));

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

            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });
        });

        // File hosting
        var path = Path.Combine(configuration["File:Location"], configuration["File:Destination"]);

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
