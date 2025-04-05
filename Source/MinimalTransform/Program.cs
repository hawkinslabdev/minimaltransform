using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using MinimalTransform.Routes;
using MinimalTransform.Middleware;
using MinimalTransform.Classes;
using MinimalTransform.Configurations;
using dotenv.net;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/minimaltransform-.log",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10 * 1024 * 1024,
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 10,
        buffered: true,
        flushToDiskInterval: TimeSpan.FromSeconds(30))
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Filter.ByExcluding(logEvent =>
        logEvent.Properties.ContainsKey("RequestPath") &&
        (logEvent.Properties["RequestPath"].ToString().Contains("/swagger") ||
         logEvent.Properties["RequestPath"].ToString().Contains("/favicon.ico")))
    .CreateLogger();

Log.Information("Starting MinimalTransform application");

// Load environment variables - make sure this is at the top, before we use env vars
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 2));

// Log the environment variables for debugging (don't include in production)
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    var apiKeys = Environment.GetEnvironmentVariable("API_KEYS");
    var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
    Log.Debug("API_KEYS: {ApiKeys}", apiKeys);
    Log.Debug("ALLOWED_ORIGINS: {AllowedOrigins}", allowedOrigins);
}

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCors(options =>
{
    options.AddPolicy("RestrictedOrigins", policy =>
    {
        var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries) 
            ?? Array.Empty<string>();
            
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Configure static file middleware
builder.Services.Configure<StaticFileMiddlewareOptions>(options =>
{
    options.BlockedFiles = new[] { "index.html", "convert.html" };
});

// Configure API key middleware
builder.Services.AddApiKeyProtection(options =>
{
    options.ApiKeys = Environment.GetEnvironmentVariable("API_KEYS")
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
        ?? Array.Empty<string>();
        
    options.AllowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
        ?? Array.Empty<string>();
});

builder.Services.AddHostedService<LogFlusher>();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var urls = app.Urls.Any() ? string.Join(", ", app.Urls) : "http://localhost:5111";
    Log.Information("🌐 Application running at: {Urls}", urls);
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("Application shutting down...");
    Log.CloseAndFlush();
});

var swaggerEnabled = builder.Configuration.GetValue<bool>("SwaggerEnabled", false);

if (swaggerEnabled)
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseCors("RestrictedOrigins");

app.UseStaticFiles();  
app.UseStaticFileProtection();  
app.UseApiKeyProtection();  

// Routes after middleware
app.MapFormatConversionRoutes();
app.MapUiRoutes();

app.Run();

public class ConversionRequest
{
    public string SourceFormat { get; set; }
    public string TargetFormat { get; set; }
    public string Content { get; set; }
}