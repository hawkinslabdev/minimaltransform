using System;

namespace MinimalTransform.Helpers;

// Helper for converting XML to CSV
public static class XmlToCsvHelper
{
    // Convert XML string to CSV string
    public static string ConvertXmlToCsv(string xmlString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                throw new ArgumentException("Invalid XML data");

            // Use the unified ConversionCsvHelper
            return ConversionCsvHelper.XmlToCsv(xmlString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting XML to CSV: {ex.Message}", ex);
        }
    }
}