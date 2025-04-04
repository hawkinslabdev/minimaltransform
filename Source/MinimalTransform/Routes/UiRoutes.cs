using Microsoft.AspNetCore.StaticFiles;

public static class UiRoutes
{
    public static void MapUiRoutes(this WebApplication app)
    {
        app.MapGet("/convert", async context =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/convert.html");
        });

        app.MapGet("/swagger", async context =>
        {
            // This route may never be reached, because this is handled before this middleware is used
            return;
        });

        app.MapGet("/", async (HttpContext context) =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
        });

        app.MapFallback(async (HttpContext context) =>
        {
            if (!context.Request.Path.Value.StartsWith("/api"))
            {
                var filePath = Path.Combine(app.Environment.WebRootPath, context.Request.Path.Value.TrimStart('/'));
                
                if (File.Exists(filePath))
                {
                    context.Response.ContentType = GetContentType(filePath);
                    await context.Response.SendFileAsync(filePath);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { 
                        error = "Resource not found",
                        success = false
                    });
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { 
                    error = "API endpoint not found",
                    success = false
                });
            }
        });
    }

    private static string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}