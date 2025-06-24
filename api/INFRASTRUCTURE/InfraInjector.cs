using APPLICATION.IRepository;
using APPLICATION.IService;
using DOMAIN.Model;
using INFRASTRUCTURE.Data;
using INFRASTRUCTURE.Repository;
using INFRASTRUCTURE.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace INFRASTRUCTURE;

public class InfraInjector
{
    public static void Inject(IServiceCollection services, ConfigurationManager configuration)
    {
        // Inject repositories
        #region REPOSITORIES
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>(); /* added by make.py */
            services.AddScoped<IRoleActionRepository, RoleActionRepository>(); /* added by make.py */
            services.AddScoped<IUserXAccessRepository, UserXAccessRepository>(); /* added by make.py */
        #endregion

        // Inject services
        #region SERVICES
            services.AddScoped<IUserService, UserService>();
			services.AddScoped<IRoleService, RoleService>(); /* added by make.py */
			services.AddScoped<IRoleActionService, RoleActionService>(); /* added by make.py */
			services.AddScoped<IUserXAccessService, UserXAccessService>(); /* added by make.py */
		#endregion

        // Identity
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider)
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Default Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 0;
            options.User.RequireUniqueEmail = true;
        });

        // Newton soft json
        services.AddControllers()
            .AddNewtonsoftJson(
                options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
    }
}