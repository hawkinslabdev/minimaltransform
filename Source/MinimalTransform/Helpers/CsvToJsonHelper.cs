using System;

namespace MinimalTransform.Helpers;

// Helper for converting CSV to JSON
public static class CsvToJsonHelper
{
    // Convert CSV string to JSON string
    public static string ConvertCsvToJson(string csvString, int indentation = 2)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            // Use the unified ConversionCsvHelper
            return ConversionCsvHelper.CsvToJson(csvString, indentation);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to JSON: {ex.Message}", ex);
        }
    }
}