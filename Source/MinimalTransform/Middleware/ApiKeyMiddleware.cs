using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace MinimalTransform.Middleware;

public class ApiKeyMiddlewareOptions
{
    public string[] ApiKeys { get; set; } = Array.Empty<string>();
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private readonly string[] _apiKeys;
    private readonly string[] _allowedOrigins;

    public ApiKeyMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyMiddleware> logger,
        IOptions<ApiKeyMiddlewareOptions> options)
    {
        _next = next;
        _logger = logger;
        _apiKeys = options.Value.ApiKeys;
        _allowedOrigins = options.Value.AllowedOrigins;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        
        // Only apply middleware to API routes
        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            // Allow swagger documentation to pass through
            if (path.StartsWith("/api/docs", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            
            // Check for:
            
            // 1. Same-origin requests
            if (IsSameOriginRequest(context))
            {
                _logger.LogDebug("API access permitted: Same-origin request");
                await _next(context);
                return;
            }
            
            // 2. Valid API key
            if (context.Request.Headers.TryGetValue("X-API-KEY", out var apiKey) && 
                _apiKeys.Contains(apiKey.ToString()))
            {
                _logger.LogDebug("API access permitted: Valid API key");
                await _next(context);
                return;
            }
            
            // 3. Allowed origin
            if (context.Request.Headers.TryGetValue("Origin", out var origin) && 
                _allowedOrigins.Contains(origin.ToString()))
            {
                _logger.LogDebug("API access permitted: Allowed origin");
                await _next(context);
                return;
            }
            
            // Access denied - return 401 Unauthorized
            _logger.LogWarning("API access denied: Unauthorized access attempt to {Path}", path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { 
                error = "Unauthorized access. API key required.",
                success = false
            });
            return;
        }

        // Non-API routes are passed through
        await _next(context);
    }
    
    // Check if the request is coming from the same origin
    private static bool IsSameOriginRequest(HttpContext context)
    {
        var referer = context.Request.Headers.Referer.ToString();
        if (string.IsNullOrEmpty(referer)) return false;
        
        var host = $"{context.Request.Scheme}://{context.Request.Host}";
        return referer.StartsWith(host);
    }
}

// Extension method for easy middleware registration
public static class ApiKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyProtection(
        this IApplicationBuilder builder)
    {
        // Register middleware
        return builder.UseMiddleware<ApiKeyMiddleware>();
    }
    
    public static IServiceCollection AddApiKeyProtection(
        this IServiceCollection services, 
        Action<ApiKeyMiddlewareOptions> configureOptions)
    {
        // Register options
        services.Configure(configureOptions);
        return services;
    }
}