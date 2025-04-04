using System;
using System.Text.Json;
using System.Text.Encodings.Web;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MinimalTransform.Helpers;

// Helper for converting YAML to JSON
public static class YamlToJsonHelper
{
    // Convert YAML string to JSON string
    public static string ConvertYamlToJson(string yamlString, int indentation = 2)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(yamlString))
                throw new ArgumentException("Invalid YAML data");
            
            // Convert YAML to object
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
                
            var yamlObject = deserializer.Deserialize<object>(yamlString);
            
            // Convert object to JSON
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = indentation > 0,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            string jsonString = JsonSerializer.Serialize(yamlObject, jsonOptions);
            
            return jsonString;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to JSON: {ex.Message}", ex);
        }
    }
}