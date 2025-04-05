using System;

namespace MinimalTransform.Helpers;

// Helper for converting CSV to XML
public static class CsvToXmlHelper
{
    // Convert CSV string to XML string
    public static string ConvertCsvToXml(string csvString, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            // Use the unified ConversionCsvHelper
            return ConversionCsvHelper.CsvToXml(csvString, indentation, rootName);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to XML: {ex.Message}", ex);
        }
    }
}