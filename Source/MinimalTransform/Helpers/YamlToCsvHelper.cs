using System;

namespace MinimalTransform.Helpers;

// Helper for converting YAML to CSV (via JSON)
public static class YamlToCsvHelper
{
    // Convert YAML string to CSV string
    public static string ConvertYamlToCsv(string yamlString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(yamlString))
                throw new ArgumentException("Invalid YAML data");
            
            // Convert YAML to JSON first (without indentation for intermediate format)
            string jsonString = YamlToJsonHelper.ConvertYamlToJson(yamlString, 0);
            
            // Convert JSON to CSV
            string csv = JsonToCsvHelper.ConvertJsonToCsv(jsonString);
            
            return csv;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to CSV: {ex.Message}", ex);
        }
    }
}