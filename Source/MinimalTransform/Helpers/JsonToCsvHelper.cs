using System;

namespace MinimalTransform.Helpers;

// Helper for converting JSON to CSV
public static class JsonToCsvHelper
{
    // Convert JSON string to CSV string
    public static string ConvertJsonToCsv(string jsonString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                throw new ArgumentException("Invalid JSON data");

            // Use the unified ConversionCsvHelper
            return ConversionCsvHelper.JsonToCsv(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting JSON to CSV: {ex.Message}", ex);
        }
    }
}