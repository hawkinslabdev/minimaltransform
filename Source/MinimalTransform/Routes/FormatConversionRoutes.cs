using System.Text;
using System.Text.Json;
using MinimalTransform.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

public static class FormatConversionRoutes
{
    public static void MapFormatConversionRoutes(this WebApplication app)
    {
        // CSV to JSON endpoint
        app.MapPost("/api/convert/csv-to-json", async (HttpRequest request, [FromQuery] int indentation = 2) =>
        {
            try
            {
                string csvString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(csvString))
                    return Results.BadRequest("Invalid CSV data");

                string jsonString = CsvToJsonHelper.ConvertCsvToJson(csvString, indentation);
                return Results.Content(jsonString, contentType: CommonHelper.GetContentType("json"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting CSV to JSON: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts CSV to JSON", "text/csv", "json", "File Conversion"));

        // CSV to XML endpoint
        app.MapPost("/api/convert/csv-to-xml", async (HttpRequest request, [FromQuery] int indentation = 2, [FromQuery] string rootName = "root") =>
        {
            try
            {
                string csvString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(csvString))
                    return Results.BadRequest("Invalid CSV data");

                string xml = CsvToXmlHelper.ConvertCsvToXml(csvString, indentation, rootName);
                return Results.Content(xml, contentType: CommonHelper.GetContentType("xml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting CSV to XML: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts CSV to XML", "text/csv", "xml", "File Conversion"));

        // CSV to YAML endpoint
        app.MapPost("/api/convert/csv-to-yaml", async (HttpRequest request) =>
        {
            try
            {
                string csvString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(csvString))
                    return Results.BadRequest("Invalid CSV data");

                string yaml = CsvToYamlHelper.ConvertCsvToYaml(csvString);
                return Results.Content(yaml, contentType: CommonHelper.GetContentType("yaml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting CSV to YAML: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts CSV to YAML", "text/csv", "yaml", "File Conversion"));

        // JSON to CSV endpoint
        app.MapPost("/api/convert/json-to-csv", async (HttpRequest request) =>
        {
            try
            {
                string jsonString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(jsonString))
                    return Results.BadRequest("Invalid JSON data");

                string csv = JsonToCsvHelper.ConvertJsonToCsv(jsonString);
                return Results.Content(csv, contentType: CommonHelper.GetContentType("csv"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting JSON to CSV: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts JSON to CSV", "application/json", "csv", "File Conversion"));

        // JSON to XML endpoint
        app.MapPost("/api/convert/json-to-xml", async (HttpRequest request, [FromQuery] int indentation = 2, [FromQuery] string rootName = "root") =>
        {
            try
            {
                string jsonString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(jsonString))
                    return Results.BadRequest("Invalid JSON data");

                var node = JsonToXmlHelper.ConvertJsonToXml(jsonString, rootName);
                var settings = CommonHelper.GetXmlSettings(indentation);

                var sb = new StringBuilder();
                using (var writer = System.Xml.XmlWriter.Create(sb, settings))
                {
                    node.WriteTo(writer);
                }

                return Results.Content(sb.ToString(), contentType: CommonHelper.GetContentType("xml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting JSON to XML: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts JSON to XML", "application/json", "xml", "File Conversion"));

        // JSON to YAML endpoint
        app.MapPost("/api/convert/json-to-yaml", async (HttpRequest request) =>
        {
            try
            {
                string jsonString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(jsonString))
                    return Results.BadRequest("Invalid JSON data");

                string yaml = JsonToYamlHelper.ConvertJsonToYaml(jsonString);
                return Results.Content(yaml, contentType: CommonHelper.GetContentType("yaml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting JSON to YAML: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts JSON to YAML", "application/json", "yaml", "File Conversion"));

        // XML to CSV endpoint
        app.MapPost("/api/convert/xml-to-csv", async (HttpRequest request) =>
        {
            try
            {
                string xmlString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(xmlString))
                    return Results.BadRequest("Invalid XML data");

                string csv = XmlToCsvHelper.ConvertXmlToCsv(xmlString);
                return Results.Content(csv, contentType: CommonHelper.GetContentType("csv"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting XML to CSV: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts XML to CSV", "application/xml", "csv", "File Conversion"));

        // XML to JSON endpoint
        app.MapPost("/api/convert/xml-to-json", async (HttpRequest request, [FromQuery] int indentation = 2) =>
        {
            try
            {
                string xmlString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(xmlString))
                    return Results.BadRequest("Invalid XML data");

                string jsonString = XmlToJsonHelper.ConvertXmlToJson(xmlString, indentation);
                return Results.Content(jsonString, contentType: CommonHelper.GetContentType("json"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting XML to JSON: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts XML to JSON", "application/xml", "json", "File Conversion"));

        // XML to YAML endpoint
        app.MapPost("/api/convert/xml-to-yaml", async (HttpRequest request) =>
        {
            try
            {
                string xmlString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(xmlString))
                    return Results.BadRequest("Invalid XML data");

                string yaml = XmlToYamlHelper.ConvertXmlToYaml(xmlString);
                return Results.Content(yaml, contentType: CommonHelper.GetContentType("yaml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting XML to YAML: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts XML to YAML", "application/xml", "yaml", "File Conversion"));

        // YAML to CSV endpoint
        app.MapPost("/api/convert/yaml-to-csv", async (HttpRequest request) =>
        {
            try
            {
                string yamlString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(yamlString))
                    return Results.BadRequest("Invalid YAML data");

                string csv = YamlToCsvHelper.ConvertYamlToCsv(yamlString);
                return Results.Content(csv, contentType: CommonHelper.GetContentType("csv"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting YAML to CSV: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts YAML to CSV", "application/x-yaml", "csv", "File Conversion"));

        // YAML to JSON endpoint
        app.MapPost("/api/convert/yaml-to-json", async (HttpRequest request, [FromQuery] int indentation = 2) =>
        {
            try
            {
                string yamlString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(yamlString))
                    return Results.BadRequest("Invalid YAML data");

                string jsonString = YamlToJsonHelper.ConvertYamlToJson(yamlString, indentation);
                return Results.Content(jsonString, contentType: CommonHelper.GetContentType("json"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting YAML to JSON: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts YAML to JSON", "application/x-yaml", "json", "File Conversion"));

        // YAML to XML endpoint
        app.MapPost("/api/convert/yaml-to-xml", async (HttpRequest request, [FromQuery] int indentation = 2, [FromQuery] string rootName = "root") =>
        {
            try
            {
                string yamlString = await ReadBodyAsync(request);
                if (!CommonHelper.IsValidInput(yamlString))
                    return Results.BadRequest("Invalid YAML data");

                string xml = YamlToXmlHelper.ConvertYamlToXml(yamlString, indentation, rootName);
                return Results.Content(xml, contentType: CommonHelper.GetContentType("xml"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Error converting YAML to XML: {ex.Message}");
            }
        }).WithOpenApi(operation => BuildOpenApi("Converts YAML to XML", "application/x-yaml", "xml", "File Conversion"));
    }

    private static async Task<string> ReadBodyAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    private static OpenApiOperation BuildOpenApi(string description, string requestContentType, string responseContentType, string tag)
    {
        var operation = new OpenApiOperation
        {
            Description = description,
            RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [requestContentType] = new OpenApiMediaType()
                }
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

        operation.Tags = new List<OpenApiTag> 
        { 
            new OpenApiTag { Name = tag } 
        };

        return operation;
    }
}