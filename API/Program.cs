using System.Runtime.InteropServices;
using API;
using API.Attributes;
using APPLICATION;
using INFRASTRUCTURE;
using Microsoft.Extensions.FileProviders;

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
    c.DocumentTitle = app.Configuration["App"];
    c.SwaggerEndpoint("v1/swagger.json", app.Configuration["App"]);
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();