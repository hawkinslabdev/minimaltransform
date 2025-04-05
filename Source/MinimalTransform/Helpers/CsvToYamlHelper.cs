using System;

namespace MinimalTransform.Helpers;

// Helper for converting CSV to YAML
public static class CsvToYamlHelper
{
    // Convert CSV string to YAML string
    public static string ConvertCsvToYaml(string csvString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            // Use the unified ConversionCsvHelper
            return ConversionCsvHelper.CsvToYaml(csvString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to YAML: {ex.Message}", ex);
        }
    }
}