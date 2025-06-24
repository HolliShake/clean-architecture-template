using System.Runtime.InteropServices;
using APPLICATION;
using INFRASTRUCTURE;
using INFRASTRUCTURE.ExceptionHandler;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace API;

/// <summary>
/// Program class for the API.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main method for the API.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Configuration
        builder.Services.AddSingleton(builder.Configuration);

        // Add services to the container.
        // Infra dependency injection
        InfraInjector.Inject(builder.Services, builder.Configuration);

        // App dependency injection
        AppInjector.Inject(builder.Services, builder.Configuration);

        // Api dependency injection
        ApiInjector.Inject(builder.Services, builder.Configuration);

        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                    return new BadRequestObjectResult(new { message = "Invalid model state", errors = errors });
                };
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = long.Parse(builder.Configuration["File:Limit"]); // (1024 * 1024 * 2048) 800 MB
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = long.Parse(builder.Configuration["File:Limit"]); // (1024 * 1024 * 2048) 800 MB
        });

        var app = builder.Build();

        app.UseDeveloperExceptionPage();

        app.UseStatusCodePages();
        app.UseExceptionHandler(exceptionhandler =>
        {
            exceptionhandler.Run(async context =>
            {
                context.Response.ContentType = "text/plain";

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature == null)
                {
                    return;
                }

                context.Response.StatusCode = exceptionHandlerPathFeature.Error switch
                {
                    Error400Exception => StatusCodes.Status400BadRequest,
                    Error401Exception => StatusCodes.Status401Unauthorized,
                    Error404Exception => StatusCodes.Status404NotFound,
                    Error500Exception => StatusCodes.Status500InternalServerError,
                    _ => StatusCodes.Status500InternalServerError
                };

                await context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
            });
        });

        app.UseCors(x => x
            .WithOrigins("https://localhost:5173", "https://127.0.0.1:5173", "https://localhost:4000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("*")
            .SetIsOriginAllowed(_ => true) // allow any origin
            .AllowCredentials()); // allow credential

        // Hosting
        var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(builder.Configuration["File:LocationWIN32"], builder.Configuration["File:DestinationWIN32"])
            : Path.Combine(builder.Configuration["File:LocationLINUX"], builder.Configuration["File:DestinationLINUX"]);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(path),
            RequestPath = builder.Configuration["File:Request"]
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            var title = app.Configuration["App"];

            c.DocumentTitle = title;

            if (app.Environment.IsProduction() || app.Environment.IsDevelopment())
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", title);
            }
            else
            {
                c.SwaggerEndpoint($"/dev/swagger/v1/swagger.json", title);
            }

            c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
        });
        
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}