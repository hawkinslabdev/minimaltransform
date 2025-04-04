using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MinimalTransform.Helpers;

// Helper for converting JSON to YAML
public static class JsonToYamlHelper
{
    // Convert JSON string to YAML string
    public static string ConvertJsonToYaml(string jsonString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                throw new ArgumentException("Invalid JSON data");
            
            // Parse JSON to dictionary
            var jsonDoc = JsonNode.Parse(jsonString);
            var objectGraph = ConvertJsonNodeToDictionary(jsonDoc);
            
            // Convert dictionary to YAML
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithIndentedSequences()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();
                
            var yaml = serializer.Serialize(objectGraph);
            
            return yaml;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting JSON to YAML: {ex.Message}", ex);
        }
    }

    // Recursively convert JsonNode to Dictionary/List structure
    private static object ConvertJsonNodeToDictionary(JsonNode node)
    {
        if (node is null)
            return null;
            
        if (node is JsonObject obj)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var property in obj)
            {
                dictionary[property.Key] = ConvertJsonNodeToDictionary(property.Value);
            }
            return dictionary;
        }
        else if (node is JsonArray array)
        {
            var list = new List<object>();
            foreach (var item in array)
            {
                list.Add(ConvertJsonNodeToDictionary(item));
            }
            return list;
        }
        else if (node is JsonValue value)
        {
            return GetValueFromJsonValue(value);
        }
        
        return null;
    }
    
    private static object GetValueFromJsonValue(JsonValue value)
    {
        // Try to infer the correct type based on the JsonValue
        if (value.TryGetValue(out bool boolValue))
            return boolValue;
        if (value.TryGetValue(out int intValue))
            return intValue;
        if (value.TryGetValue(out double doubleValue))
            return doubleValue;
        if (value.TryGetValue(out string stringValue))
            return stringValue;
            
        // If we can't determine the type, convert to string as fallback
        return value.ToString();
    }
}