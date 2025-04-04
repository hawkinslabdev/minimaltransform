using System;

namespace MinimalTransform.Helpers;

// Helper for converting CSV to YAML (via JSON)
public static class CsvToYamlHelper
{
    // Convert CSV string to YAML string
    public static string ConvertCsvToYaml(string csvString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            // Convert CSV to JSON (no indent for intermediate format)
            var jsonString = CsvToJsonHelper.ConvertCsvToJson(csvString, indentation: 0);

            // Convert JSON to YAML
            var yaml = JsonToYamlHelper.ConvertJsonToYaml(jsonString);

            return yaml;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to YAML: {ex.Message}", ex);
        }
    }
}
