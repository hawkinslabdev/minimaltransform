
using System.Text.Json;
using MinimalTransform.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;

public static class FormatConversionRoutes
{
    public static void MapFormatConversionRoutes(this WebApplication app)
    {
        // CSV to JSON endpoint
        app.MapPost("/api/convert/csv-to-json", ([FromBody] string csvString, [FromQuery] int indentation = 2) =>
        {
            if (!CommonHelper.IsValidInput(csvString))
                return Results.BadRequest("Invalid CSV data");

            string jsonString = FormatConverter.CsvToJson(csvString, indentation);
            return Results.Content(jsonString, contentType: CommonHelper.GetContentType("json"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts CSV to JSON",
            new[] { "text/csv", "text/plain" },
            "json",
            "File Conversion"
        ));

        // CSV to XML endpoint
        app.MapPost("/api/convert/csv-to-xml", ([FromBody] string csvString, [FromQuery] int indentation = 2, [FromQuery] string rootName = "root") =>
        {
            if (!CommonHelper.IsValidInput(csvString))
                return Results.BadRequest("Invalid CSV data");

            string xml = FormatConverter.CsvToXml(csvString, indentation, rootName);
            return Results.Content(xml, contentType: CommonHelper.GetContentType("xml"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts CSV to XML",
            new[] { "text/csv", "text/plain" },
            "xml",
            "File Conversion"
        ));

        // CSV to YAML endpoint
        app.MapPost("/api/convert/csv-to-yaml", ([FromBody] string csvString) =>
        {
            if (!CommonHelper.IsValidInput(csvString))
                return Results.BadRequest("Invalid CSV data");

            string yaml = FormatConverter.CsvToYaml(csvString);
            return Results.Content(yaml, contentType: CommonHelper.GetContentType("yaml"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts CSV to YAML",
            new[] { "text/csv", "text/plain" },
            "yaml",
            "File Conversion"
        ));

        // JSON to CSV endpoint
        app.MapPost("/api/convert/json-to-csv", ([FromBody] string jsonString) =>
        {
            if (!CommonHelper.IsValidInput(jsonString))
                return Results.BadRequest("Invalid JSON data");

            string csv = FormatConverter.JsonToCsv(jsonString);
            return Results.Content(csv, contentType: CommonHelper.GetContentType("csv"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts JSON to CSV",
            new[] { "application/json", "text/plain" },
            "csv",
            "File Conversion"
        ));

        // JSON to XML endpoint
        app.MapPost("/api/convert/json-to-xml", ([FromBody] string jsonString, [FromQuery] int indentation = 2, [FromQuery] string rootName = "root") =>
        {
            if (!CommonHelper.IsValidInput(jsonString))
                return Results.BadRequest("Invalid JSON data");

            string xml = FormatConverter.JsonToXml(jsonString, indentation, rootName);
            return Results.Content(xml, contentType: CommonHelper.GetContentType("xml"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts JSON to XML",
            new[] { "application/json", "text/plain" },
            "xml",
            "File Conversion"
        ));

        // JSON to YAML endpoint
        app.MapPost("/api/convert/json-to-yaml", ([FromBody] string jsonString) =>
        {
            if (!CommonHelper.IsValidInput(jsonString))
                return Results.BadRequest("Invalid JSON data");

            string yaml = FormatConverter.JsonToYaml(jsonString);
            return Results.Content(yaml, contentType: CommonHelper.GetContentType("yaml"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts JSON to YAML",
            new[] { "application/json", "text/plain" },
            "yaml",
            "File Conversion"
        ));

        // XML to CSV endpoint
        app.MapPost("/api/convert/xml-to-csv", ([FromBody] string xmlString) =>
        {
            if (!CommonHelper.IsValidInput(xmlString))
                return Results.BadRequest("Invalid XML data");

            string csv = FormatConverter.XmlToCsv(xmlString);
            return Results.Content(csv, contentType: CommonHelper.GetContentType("csv"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts XML to CSV",
            new[] { "application/xml", "text/xml", "text/plain" },
            "csv",
            "File Conversion"
        ));

        // XML to JSON endpoint
        app.MapPost("/api/convert/xml-to-json", ([FromBody] string xmlString, [FromQuery] int indentation = 2) =>
        {
            if (!CommonHelper.IsValidInput(xmlString))
                return Results.BadRequest("Invalid XML data");

            string jsonString = FormatConverter.XmlToJson(xmlString, indentation);
            return Results.Content(jsonString, contentType: CommonHelper.GetContentType("json"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts XML to JSON",
            new[] { "application/xml", "text/xml", "text/plain" },
            "json",
            "File Conversion"
        ));

        // XML to YAML endpoint
        app.MapPost("/api/convert/xml-to-yaml", ([FromBody] string xmlString) =>
        {
            if (!CommonHelper.IsValidInput(xmlString))
                return Results.BadRequest("Invalid XML data");

            string yaml = FormatConverter.XmlToYaml(xmlString);
            return Results.Content(yaml, contentType: CommonHelper.GetContentType("yaml"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts XML to YAML",
            new[] { "application/xml", "text/xml", "text/plain" },
            "yaml",
            "File Conversion"
        ));

        // YAML to CSV endpoint
        app.MapPost("/api/convert/yaml-to-csv", ([FromBody] string yamlString) =>
        {
            if (!CommonHelper.IsValidInput(yamlString))
                return Results.BadRequest("Invalid YAML data");

            string csv = FormatConverter.YamlToCsv(yamlString);
            return Results.Content(csv, contentType: CommonHelper.GetContentType("csv"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts YAML to CSV",
            new[] { "application/x-yaml", "text/yaml", "text/plain" },
            "csv",
            "File Conversion"
        ));

        // YAML to JSON endpoint
        app.MapPost("/api/convert/yaml-to-json", ([FromBody] string yamlString, [FromQuery] int indentation = 2) =>
        {
            if (!CommonHelper.IsValidInput(yamlString))
                return Results.BadRequest("Invalid YAML data");

            string jsonString = FormatConverter.YamlToJson(yamlString, indentation);
            return Results.Content(jsonString, contentType: CommonHelper.GetContentType("json"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts YAML to JSON",
            new[] { "application/x-yaml", "text/yaml", "text/plain" },
            "json",
            "File Conversion"
        ));

        // YAML to XML endpoint
        app.MapPost("/api/convert/yaml-to-xml", ([FromBody] string yamlString, [FromQuery] int indentation = 2, [FromQuery] string rootName = "root") =>
        {
            if (!CommonHelper.IsValidInput(yamlString))
                return Results.BadRequest("Invalid YAML data");

            string xml = FormatConverter.YamlToXml(yamlString, indentation, rootName);
            return Results.Content(xml, contentType: CommonHelper.GetContentType("xml"));
        }).WithOpenApi(operation => BuildOpenApiWithMultipleContentTypes(
            "Converts YAML to XML",
            new[] { "application/x-yaml", "text/yaml", "text/plain" },
            "xml",
            "File Conversion"
        ));

        // Auto-detect and convert endpoint
        app.MapPost("/api/convert/auto", ([FromBody] string inputData, [FromQuery] string targetFormat, [FromQuery] int indentation = 2, [FromQuery] string rootName = "root") =>
        {
            if (!CommonHelper.IsValidInput(inputData))
                return Results.BadRequest("Invalid input data");

            targetFormat = targetFormat?.ToLowerInvariant();
            if (string.IsNullOrEmpty(targetFormat) || !new[] { "json", "xml", "yaml", "csv" }.Contains(targetFormat))
                return Results.BadRequest("Invalid target format. Supported formats: json, xml, yaml, csv");

            string result = FormatConverter.AutoConvert(inputData, targetFormat, indentation, rootName);
            return Results.Content(result, contentType: CommonHelper.GetContentType(targetFormat));
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
        });
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
                ["400"] = new OpenApiResponse { Description = "Bad Request" }
            }
        };

        foreach (var contentType in requestContentTypes)
        {
            operation.RequestBody.Content.Add(contentType, new OpenApiMediaType());
        }

        operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = tag } };
        return operation;
    }
}
