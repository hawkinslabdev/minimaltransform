using System;

namespace MinimalTransform.Helpers;

// Helper for converting YAML to CSV
public static class YamlToCsvHelper
{
    // Convert YAML string to CSV string
    public static string ConvertYamlToCsv(string yamlString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(yamlString))
                throw new ArgumentException("Invalid YAML data");
            
            // Use the unified ConversionCsvHelper
            return ConversionCsvHelper.YamlToCsv(yamlString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to CSV: {ex.Message}", ex);
        }
    }
}