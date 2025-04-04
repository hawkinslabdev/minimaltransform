using System;
using System.Text;
using System.Xml;

namespace MinimalTransform.Helpers;

// Helper for converting CSV to XML (via JSON)
public static class CsvToXmlHelper
{
    // Convert CSV string to XML string
    public static string ConvertCsvToXml(string csvString, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            // Convert CSV → JSON (no indent for intermediate)
            var jsonString = CsvToJsonHelper.ConvertCsvToJson(csvString, indentation: 0);

            // Convert JSON → XmlNode
            var xmlNode = JsonToXmlHelper.ConvertJsonToXml(jsonString, rootName);

            // Format XML with indentation
            var settings = JsonToXmlHelper.GetXmlSettings(indentation);

            using var sw = new StringWriterWithEncoding(Encoding.UTF8);
            using var writer = XmlWriter.Create(sw, settings);
            xmlNode.WriteTo(writer);
            writer.Flush();

            return sw.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to XML: {ex.Message}", ex);
        }
    }

    // Optional: override StringWriter to force UTF-8 encoding
    private sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriterWithEncoding(Encoding encoding) => _encoding = encoding;

        public override Encoding Encoding => _encoding;
    }
}

