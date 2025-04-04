using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MinimalTransform.Configurations
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Minimal Transform API",
                    Version = "v1",
                    Description = "API for file format transformation"
                });

                options.TagActionsBy(api =>
                {
                    var tag = api.GroupName ?? api.RelativePath?.Split('/').FirstOrDefault() ?? "Default";
                    return new[] { tag.Replace("-", " ").Replace("_", " ") };
                });

                options.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.ActionDescriptor.DisplayName?.Split('.').Last()?.Replace("()", "");
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

        }

        public static void UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal Transform API");
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                options.DefaultModelsExpandDepth(1);
            });
        }
    }
}
