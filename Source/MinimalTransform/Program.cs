using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using MinimalTransform.Middleware;
using MinimalTransform.Classes;
using MinimalTransform.Configurations;

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

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.Configure<StaticFileMiddlewareOptions>(options =>
{
    options.BlockedFiles = new[] { "index.html", "convert.html" };
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

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(); 
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseStaticFileProtection();

app.MapFormatConversionRoutes();
app.MapUiRoutes();

app.Run();

public class ConversionRequest
{
    public string SourceFormat { get; set; }
    public string TargetFormat { get; set; }
    public string Content { get; set; }
}
