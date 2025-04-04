using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MinimalTransform.Middleware;

public class StaticFileMiddlewareOptions
{
    public string[] BlockedFiles { get; set; } = Array.Empty<string>();
}

public class StaticFileMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _blockedFiles;

    public StaticFileMiddleware(
        RequestDelegate next, 
        IOptions<StaticFileMiddlewareOptions> options)
    {
        _next = next;
        _blockedFiles = options.Value.BlockedFiles;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        
        // Check if the request is for one of our blocked files
        bool isBlocked = _blockedFiles.Any(file => 
            path.Equals($"/{file}", StringComparison.OrdinalIgnoreCase) || 
            path.EndsWith($"/{file}", StringComparison.OrdinalIgnoreCase));
            
        if (isBlocked)
        {
            // Return 404 for direct access attempts
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // Continue with the pipeline
        await _next(context);
    }
}

// Extension method for easy middleware registration
public static class StaticFileMiddlewareExtensions
{
    public static IApplicationBuilder UseStaticFileProtection(
        this IApplicationBuilder builder)
    {
        // Register middleware
        return builder.UseMiddleware<StaticFileMiddleware>();
    }
}