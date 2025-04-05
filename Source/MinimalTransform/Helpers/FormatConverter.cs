using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.Json;

namespace MinimalTransform.Helpers;

/// <summary>
/// Unified format converter class that provides direct conversion between various data formats
/// </summary>
public static class FormatConverter
{
    #region JSON Conversions

    /// <summary>
    /// Convert JSON to XML
    /// </summary>
    public static string JsonToXml(string jsonString, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (!CommonHelper.IsValidInput(jsonString))
                throw new ArgumentException("Invalid JSON data");

            var node = JsonToXmlHelper.ConvertJsonToXml(jsonString, rootName);
            var settings = CommonHelper.GetXmlSettings(indentation);

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                node.WriteTo(writer);
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting JSON to XML: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert JSON to YAML
    /// </summary>
    public static string JsonToYaml(string jsonString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(jsonString))
                throw new ArgumentException("Invalid JSON data");

            return JsonToYamlHelper.ConvertJsonToYaml(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting JSON to YAML: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert JSON to CSV
    /// </summary>
    public static string JsonToCsv(string jsonString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(jsonString))
                throw new ArgumentException("Invalid JSON data");

            return ConversionCsvHelper.JsonToCsv(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting JSON to CSV: {ex.Message}", ex);
        }
    }

    #endregion

    #region XML Conversions

    /// <summary>
    /// Convert XML to JSON
    /// </summary>
    public static string XmlToJson(string xmlString, int indentation = 2)
    {
        try
        {
            if (!CommonHelper.IsValidInput(xmlString))
                throw new ArgumentException("Invalid XML data");

            return XmlToJsonHelper.ConvertXmlToJson(xmlString, indentation);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting XML to JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert XML to YAML
    /// </summary>
    public static string XmlToYaml(string xmlString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(xmlString))
                throw new ArgumentException("Invalid XML data");

            return XmlToYamlHelper.ConvertXmlToYaml(xmlString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting XML to YAML: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert XML to CSV
    /// </summary>
    public static string XmlToCsv(string xmlString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                throw new ArgumentException("Invalid XML data");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            // Convert XML to Dictionary
            var root = doc.DocumentElement;
            var dict = XmlToDictionary(root);

            // Serialize to JSON (optional: for debugging or intermediate inspection)
            string jsonString = JsonSerializer.Serialize(dict);

            // Convert to CSV
            return JsonToCsvHelper.ConvertJsonToCsv(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting XML to CSV: {ex.Message}", ex);
        }
    }

    private static object XmlToDictionary(XmlNode node)
    {
        if (node.HasChildNodes)
        {
            // Handle text-only node
            if (node.ChildNodes.Count == 1 && node.FirstChild is XmlText)
                return node.InnerText;

            var dict = new Dictionary<string, object>();
            foreach (XmlNode child in node.ChildNodes)
            {
                var childObject = XmlToDictionary(child);

                if (dict.ContainsKey(child.Name))
                {
                    // Promote to List if duplicate key
                    if (dict[child.Name] is List<object> list)
                        list.Add(childObject);
                    else
                        dict[child.Name] = new List<object> { dict[child.Name], childObject };
                }
                else
                {
                    dict[child.Name] = childObject;
                }
            }
            return dict;
        }
        return node.InnerText;
    }

    #endregion

    #region YAML Conversions

    /// <summary>
    /// Convert YAML to JSON
    /// </summary>
    public static string YamlToJson(string yamlString, int indentation = 2)
    {
        try
        {
            if (!CommonHelper.IsValidInput(yamlString))
                throw new ArgumentException("Invalid YAML data");

            return YamlToJsonHelper.ConvertYamlToJson(yamlString, indentation);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert YAML to XML
    /// </summary>
    public static string YamlToXml(string yamlString, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (!CommonHelper.IsValidInput(yamlString))
                throw new ArgumentException("Invalid YAML data");

            return YamlToXmlHelper.ConvertYamlToXml(yamlString, indentation, rootName);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to XML: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert YAML to CSV
    /// </summary>
    public static string YamlToCsv(string yamlString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(yamlString))
                throw new ArgumentException("Invalid YAML data");

            return ConversionCsvHelper.YamlToCsv(yamlString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to CSV: {ex.Message}", ex);
        }
    }

    #endregion

    #region CSV Conversions

    /// <summary>
    /// Convert CSV to JSON
    /// </summary>
    public static string CsvToJson(string csvString, int indentation = 2)
    {
        try
        {
            if (!CommonHelper.IsValidInput(csvString))
                throw new ArgumentException("Invalid CSV data");

            return ConversionCsvHelper.CsvToJson(csvString, indentation);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert CSV to XML
    /// </summary>
    public static string CsvToXml(string csvString, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (!CommonHelper.IsValidInput(csvString))
                throw new ArgumentException("Invalid CSV data");

            return ConversionCsvHelper.CsvToXml(csvString, indentation, rootName);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to XML: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert CSV to YAML
    /// </summary>
    public static string CsvToYaml(string csvString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(csvString))
                throw new ArgumentException("Invalid CSV data");

            return ConversionCsvHelper.CsvToYaml(csvString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to YAML: {ex.Message}", ex);
        }
    }

    #endregion

    #region Advanced Conversions

    /// <summary>
    /// Detect format and convert to target format
    /// </summary>
    public static string AutoConvert(string input, string targetFormat, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (!CommonHelper.IsValidInput(input))
                throw new ArgumentException("Invalid input data");

            string sourceFormat = DetectFormat(input);
            if (string.IsNullOrEmpty(sourceFormat))
                throw new ArgumentException("Could not detect input format");

            // Skip if already in target format
            if (sourceFormat.Equals(targetFormat, StringComparison.OrdinalIgnoreCase))
                return input;

            // Convert based on source/target format combination
            return (sourceFormat.ToLowerInvariant(), targetFormat.ToLowerInvariant()) switch
            {
                ("json", "xml") => JsonToXml(input, indentation, rootName),
                ("json", "yaml") => JsonToYaml(input),
                ("json", "csv") => JsonToCsv(input),
                
                ("xml", "json") => XmlToJson(input, indentation),
                ("xml", "yaml") => XmlToYaml(input),
                ("xml", "csv") => XmlToCsv(input),
                
                ("yaml", "json") => YamlToJson(input, indentation),
                ("yaml", "xml") => YamlToXml(input, indentation, rootName),
                ("yaml", "csv") => YamlToCsv(input),
                
                ("csv", "json") => CsvToJson(input, indentation),
                ("csv", "xml") => CsvToXml(input, indentation, rootName),
                ("csv", "yaml") => CsvToYaml(input),
                
                _ => throw new ArgumentException($"Conversion from {sourceFormat} to {targetFormat} is not supported")
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during auto-conversion: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Attempt to detect format of the input string
    /// </summary>
    public static string DetectFormat(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        input = input.Trim();

        // Check for XML
        if ((input.StartsWith("<?xml") || input.StartsWith("<")) && input.Contains("</"))
        {
            try
            {
                XDocument.Parse(input);
                return "xml";
            }
            catch
            {
                // Not valid XML
            }
        }

        // Check for JSON
        if ((input.StartsWith("{") && input.EndsWith("}")) || 
            (input.StartsWith("[") && input.EndsWith("]")))
        {
            try
            {
                System.Text.Json.JsonDocument.Parse(input);
                return "json";
            }
            catch
            {
                // Not valid JSON
            }
        }

        // Check for YAML
        if (input.Contains(":") && !input.Contains("<") && !input.Contains("{") && !input.Contains("["))
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
                deserializer.Deserialize<object>(input);
                return "yaml";
            }
            catch
            {
                // Not valid YAML
            }
        }

        // Check for CSV (if it has commas and new lines)
        if (input.Contains(",") && input.Contains("\n"))
        {
            // Simple check: if it has multiple lines and commas
            return "csv";
        }

        // Could not detect format
        return null;
    }

    #endregion
}