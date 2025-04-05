using System.Text.Json;
using MinimalTransform.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using System.Text;
using Serilog;

namespace MinimalTransform.Routes;  

public static class FormatConversionRoutes
{
    public static void MapFormatConversionRoutes(this WebApplication app)
    {
        var apiGroup = app.MapGroup("/api/convert");

        // CSV to JSON endpoint
        apiGroup.MapPost("/csv-to-json", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string csvString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(csvString))
                    return Results.BadRequest("Invalid CSV data");
                
                // Get indentation from query
                int indentation = GetIndentationParam(context.Request);

                string jsonString = FormatConverter.CsvToJson(csvString, indentation);
                
                // Return the response directly
                 return Results.Content(jsonString, CommonHelper.GetContentType("json"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts CSV to JSON",
            new[] { "text/csv", "text/plain" },
            "json",
            "File Conversion"
        )).Accepts<string>("text/csv");  // This adds schema information for Swagger

        // CSV to XML endpoint
        apiGroup.MapPost("/csv-to-xml", async (HttpContext context) =>
        {
            try
            {
                string csvString = await ReadBodyAsText(context.Request);

                if (!CommonHelper.IsValidInput(csvString))
                    return Results.BadRequest("Invalid CSV data");

                int indentation = GetIndentationParam(context.Request);
                string rootName = GetRootNameParam(context.Request);

                string xml = FormatConverter.CsvToXml(csvString, indentation, rootName);

                return Results.Content(xml, CommonHelper.GetContentType("xml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts CSV to XML",
            new[] { "text/csv", "text/plain" },
            "xml",
            "File Conversion"
        )).Accepts<string>("text/csv");

        // CSV to YAML endpoint
        apiGroup.MapPost("/csv-to-yaml", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string csvString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(csvString))
                    return Results.BadRequest("Invalid CSV data");

                string yaml = FormatConverter.CsvToYaml(csvString);
                
                // Return the response
                 return Results.Content(yaml, CommonHelper.GetContentType("yaml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts CSV to YAML",
            new[] { "text/csv", "text/plain" },
            "yaml",
            "File Conversion"
        )).Accepts<string>("text/csv");

        // JSON to CSV endpoint
        apiGroup.MapPost("/json-to-csv", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string jsonString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(jsonString))
                    return Results.BadRequest("Invalid JSON data");

                string csv = FormatConverter.JsonToCsv(jsonString);
                
                // Return the response
                 return Results.Content(csv, CommonHelper.GetContentType("csv"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts JSON to CSV",
            new[] { "application/json", "text/plain" },
            "csv",
            "File Conversion"
        )).Accepts<string>("application/json");

        // JSON to XML endpoint
        apiGroup.MapPost("/json-to-xml", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string jsonString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(jsonString))
                    return Results.BadRequest("Invalid JSON data");
                
                // Get parameters from query
                int indentation = GetIndentationParam(context.Request);
                string rootName = GetRootNameParam(context.Request);

                string xml = FormatConverter.JsonToXml(jsonString, indentation, rootName);
                
                // Return the response
                return Results.Content(xml, CommonHelper.GetContentType("xml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts JSON to XML",
            new[] { "application/json", "text/plain" },
            "xml",
            "File Conversion"
        )).Accepts<string>("application/json");

        // JSON to YAML endpoint
        apiGroup.MapPost("/json-to-yaml", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string jsonString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(jsonString))
                    return Results.BadRequest("Invalid JSON data");

                string yaml = FormatConverter.JsonToYaml(jsonString);
                
                // Return the response
                 return Results.Content(yaml, CommonHelper.GetContentType("yaml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts JSON to YAML",
            new[] { "application/json", "text/plain" },
            "yaml",
            "File Conversion"
        )).Accepts<string>("application/json");

        // XML to CSV endpoint
        apiGroup.MapPost("/xml-to-csv", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string xmlString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(xmlString))
                    return Results.BadRequest("Invalid XML data");

                string csv = FormatConverter.XmlToCsv(xmlString);
                
                // Return the response
                return Results.Content(csv, CommonHelper.GetContentType("csv"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts XML to CSV",
            new[] { "application/xml", "text/xml", "text/plain" },
            "csv",
            "File Conversion"
        )).Accepts<string>("application/xml");

        // XML to JSON endpoint
        apiGroup.MapPost("/xml-to-json", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string xmlString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(xmlString))
                    return Results.BadRequest("Invalid XML data");
                
                // Get indentation from query
                int indentation = GetIndentationParam(context.Request);

                string jsonString = FormatConverter.XmlToJson(xmlString, indentation);
                
                // Return the response
                 return Results.Content(jsonString, CommonHelper.GetContentType("json"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts XML to JSON",
            new[] { "application/xml", "text/xml", "text/plain" },
            "json",
            "File Conversion"
        )).Accepts<string>("application/xml");

        // XML to YAML endpoint
        apiGroup.MapPost("/xml-to-yaml", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string xmlString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(xmlString))
                    return Results.BadRequest("Invalid XML data");

                string yaml = FormatConverter.XmlToYaml(xmlString);
                
                // Return the response
                 return Results.Content(yaml, CommonHelper.GetContentType("yaml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts XML to YAML",
            new[] { "application/xml", "text/xml", "text/plain" },
            "yaml",
            "File Conversion"
        )).Accepts<string>("application/xml");

        // YAML to CSV endpoint
        apiGroup.MapPost("/yaml-to-csv", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string yamlString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(yamlString))
                    return Results.BadRequest("Invalid YAML data");

                string csv = FormatConverter.YamlToCsv(yamlString);
                
                // Return the response
                 return Results.Content(csv, CommonHelper.GetContentType("csv"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts YAML to CSV",
            new[] { "application/x-yaml", "text/yaml", "text/plain" },
            "csv",
            "File Conversion"
        )).Accepts<string>("application/x-yaml");

        // YAML to JSON endpoint
        apiGroup.MapPost("/yaml-to-json", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string yamlString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(yamlString))
                    return Results.BadRequest("Invalid YAML data");
                
                // Get indentation from query
                int indentation = GetIndentationParam(context.Request);

                string jsonString = FormatConverter.YamlToJson(yamlString, indentation);
                
                // Return the response
                 return Results.Content(jsonString, CommonHelper.GetContentType("json"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts YAML to JSON",
            new[] { "application/x-yaml", "text/yaml", "text/plain" },
            "json",
            "File Conversion"
        )).Accepts<string>("application/x-yaml");

        // YAML to XML endpoint
        apiGroup.MapPost("/yaml-to-xml", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string yamlString = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(yamlString))
                    return Results.BadRequest("Invalid YAML data");
                
                // Get parameters from query
                int indentation = GetIndentationParam(context.Request);
                string rootName = GetRootNameParam(context.Request);

                string xml = FormatConverter.YamlToXml(yamlString, indentation, rootName);
                
                // Return the response
                return Results.Content(xml, CommonHelper.GetContentType("xml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts YAML to XML",
            new[] { "application/x-yaml", "text/yaml", "text/plain" },
            "xml",
            "File Conversion"
        )).Accepts<string>("application/x-yaml");

        // Auto-detect and convert endpoint
        apiGroup.MapPost("/auto", async (HttpContext context) =>
        {
            try
            {
                // Read body as plain text
                string inputData = await ReadBodyAsText(context.Request);
                
                if (!CommonHelper.IsValidInput(inputData))
                    return Results.BadRequest("Invalid input data");
                
                // Get required target format parameter
                if (!context.Request.Query.TryGetValue("targetFormat", out var targetFormatParam))
                    return Results.BadRequest("Target format is required");
                
                string targetFormat = targetFormatParam.ToString().ToLowerInvariant();
                if (string.IsNullOrEmpty(targetFormat) || !new[] { "json", "xml", "yaml", "csv" }.Contains(targetFormat))
                    return Results.BadRequest("Invalid target format. Supported formats: json, xml, yaml, csv");
                
                // Get optional parameters
                int indentation = GetIndentationParam(context.Request);
                string rootName = GetRootNameParam(context.Request);

                string result = FormatConverter.AutoConvert(inputData, targetFormat, indentation, rootName);
                
                // Return the response
                return Results.Content(result, CommonHelper.GetContentType(targetFormat));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithOpenApi(operation =>
        {
            var op = new OpenApiOperation
            {
                Description = "Auto-detects input format and converts to specified target format",
                RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["text/plain"] = new OpenApiMediaType(),
                        ["application/json"] = new OpenApiMediaType(),
                        ["application/xml"] = new OpenApiMediaType(),
                        ["text/xml"] = new OpenApiMediaType(),
                        ["application/x-yaml"] = new OpenApiMediaType(),
                        ["text/yaml"] = new OpenApiMediaType(),
                        ["text/csv"] = new OpenApiMediaType()
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Conversion successful",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType(),
                            ["application/xml"] = new OpenApiMediaType(),
                            ["application/x-yaml"] = new OpenApiMediaType(),
                            ["text/csv"] = new OpenApiMediaType()
                        }
                    },
                    ["400"] = new OpenApiResponse { Description = "Bad Request" }
                }
            };

            op.Parameters.Add(new OpenApiParameter
            {
                Name = "targetFormat",
                In = ParameterLocation.Query,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("json"),
                        new OpenApiString("xml"),
                        new OpenApiString("yaml"),
                        new OpenApiString("csv")
                    }
                }
            });

            op.Parameters.Add(new OpenApiParameter
            {
                Name = "indentation",
                In = ParameterLocation.Query,
                Required = false,
                Schema = new OpenApiSchema { Type = "integer", Default = new OpenApiInteger(2) }
            });

            op.Parameters.Add(new OpenApiParameter
            {
                Name = "rootName",
                In = ParameterLocation.Query,
                Required = false,
                Schema = new OpenApiSchema { Type = "string", Default = new OpenApiString("root") }
            });

            op.Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Format Detection" } };

            return op;
        }).Accepts<string>("text/plain");
    }

    // Helper method to read request body as text
    private static async Task<string> ReadBodyAsText(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        
        return body;
    }

    // Helper method to get indentation parameter with default
    private static int GetIndentationParam(HttpRequest request)
    {
        int indentation = 2; // Default value
        if (request.Query.TryGetValue("indentation", out var indentParam) && 
            int.TryParse(indentParam, out var parsedIndent))
        {
            indentation = parsedIndent;
        }
        return indentation;
    }

    // Helper method to get rootName parameter with default
    private static string GetRootNameParam(HttpRequest request)
    {
        string rootName = "root"; // Default value
        if (request.Query.TryGetValue("rootName", out var rootParam))
        {
            rootName = rootParam.ToString();
        }
        return rootName;
    }

    private static OpenApiOperation BuildOpenApiWithMultipleContentTypes(
        string description,
        string[] requestContentTypes,
        string responseContentType,
        string tag)
    {
        var operation = new OpenApiOperation
        {
            Description = description,
            RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>()
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Conversion successful",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        [CommonHelper.GetContentType(responseContentType)] = new OpenApiMediaType()
                    }
                },
                ["400"] = new OpenApiResponse { Description = "Bad Request" },
                ["401"] = new OpenApiResponse { Description = "Unauthorized - API key required" }
            }
        };

        foreach (var contentType in requestContentTypes)
        {
            operation.RequestBody.Content.Add(contentType, new OpenApiMediaType());
        }

        // Add security information to the OpenAPI operation
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    new List<string>()
                }
            }
        };

        operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = tag } };
        return operation;
    }
}