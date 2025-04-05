using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace MinimalTransform.Helpers;

/// <summary>
/// Unified helper class for all CSV-related conversions,
/// leveraging the CsvHelper NuGet package.
/// </summary>
public static class ConversionCsvHelper
{
    #region CSV to JSON

    /// <summary>
    /// Convert CSV string to JSON string
    /// </summary>
    public static string CsvToJson(string csvString, int indentation = 2)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            var records = ParseCsvToRecords(csvString);
            
            var jsonOptions = CommonHelper.GetJsonSerializerOptions(indentation);
            return JsonSerializer.Serialize(records, jsonOptions);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to JSON: {ex.Message}", ex);
        }
    }

    #endregion

    #region JSON to CSV

    /// <summary>
    /// Convert JSON string to CSV string
    /// </summary>
    public static string JsonToCsv(string jsonString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                throw new ArgumentException("Invalid JSON data");

            // Parse JSON to get array of records
            var records = ParseJsonToRecords(jsonString);
            
            if (records == null || !records.Any())
                throw new ArgumentException("JSON data doesn't contain any valid records");
            
            // Convert to CSV using CsvHelper library
            return GenerateCsvFromRecords(records);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting JSON to CSV: {ex.Message}", ex);
        }
    }

    #endregion

    #region XML to CSV

    /// <summary>
    /// Convert XML string to CSV string
    /// </summary>
    public static string XmlToCsv(string xmlString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                throw new ArgumentException("Invalid XML data");

            // Parse XML to JSON format as intermediate step
            string jsonString = XmlToJsonHelper.ConvertXmlToJson(xmlString, 0);
            
            // Use JSON to CSV converter
            return JsonToCsv(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting XML to CSV: {ex.Message}", ex);
        }
    }

    #endregion

    #region CSV to XML

    /// <summary>
    /// Convert CSV string to XML string
    /// </summary>
    public static string CsvToXml(string csvString, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            // Convert to JSON first (no indentation needed for intermediate format)
            string jsonString = CsvToJson(csvString, 0);
            
            // Use JSON to XML converter
            var xmlNode = JsonToXmlHelper.ConvertJsonToXml(jsonString, rootName);
            
            // Format with proper indentation
            var settings = CommonHelper.GetXmlSettings(indentation);
            var sb = new StringBuilder();
            using (var writer = System.Xml.XmlWriter.Create(sb, settings))
            {
                xmlNode.WriteTo(writer);
            }
            
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to XML: {ex.Message}", ex);
        }
    }

    #endregion

    #region CSV to YAML

    /// <summary>
    /// Convert CSV string to YAML string
    /// </summary>
    public static string CsvToYaml(string csvString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvString))
                throw new ArgumentException("Invalid CSV data");

            // Convert to JSON first (no indentation for intermediate)
            string jsonString = CsvToJson(csvString, 0);
            
            // Convert JSON to YAML
            return JsonToYamlHelper.ConvertJsonToYaml(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to YAML: {ex.Message}", ex);
        }
    }

    #endregion

    #region YAML to CSV

    /// <summary>
    /// Convert YAML string to CSV string
    /// </summary>
    public static string YamlToCsv(string yamlString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(yamlString))
                throw new ArgumentException("Invalid YAML data");

            // Convert to JSON first
            string jsonString = YamlToJsonHelper.ConvertYamlToJson(yamlString, 0);
            
            // Convert JSON to CSV
            return JsonToCsv(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to CSV: {ex.Message}", ex);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Parse CSV to a list of dynamic records using CsvHelper
    /// </summary>
    private static List<IDictionary<string, object>> ParseCsvToRecords(string csvString)
    {
        var records = new List<IDictionary<string, object>>();

        using (var reader = new StringReader(csvString))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null,
            DetectDelimiter = true  // Auto-detect delimiter like comma, tab, etc.
        }))
        {
            try
            {
                // Read header and validate
                if (!csv.Read() || !csv.ReadHeader())
                    throw new Exception("Failed to read CSV headers");

                if (csv.HeaderRecord == null || csv.HeaderRecord.Length == 0)
                    throw new Exception("CSV has no valid headers");

                // Make headers unique if needed
                var uniqueHeaders = MakeHeadersUnique(csv.HeaderRecord);

                // Read all records
                while (csv.Read())
                {
                    var record = new ExpandoObject() as IDictionary<string, object>;

                    for (int i = 0; i < uniqueHeaders.Length; i++)
                    {
                        var header = uniqueHeaders[i];
                        string rawValue = null;
                        
                        try
                        {
                            // Try to get the field by index (safer than by name with duplicates)
                            rawValue = csv.GetField(i);
                        }
                        catch
                        {
                            // Field doesn't exist, use null
                        }

                        record[header] = ConvertCsvValue(rawValue);
                    }

                    records.Add(record);
                }
            }
            catch (CsvHelperException ex)
            {
                // Handle CsvHelper specific exceptions
                throw new Exception($"CSV parsing error: {ex.Message}");
            }
        }

        return records;
    }

    /// <summary>
    /// Make headers unique by appending numbers to duplicates
    /// </summary>
    private static string[] MakeHeadersUnique(string[] headers)
    {
        var result = new string[headers.Length];
        var counts = new Dictionary<string, int>();

        for (int i = 0; i < headers.Length; i++)
        {
            var header = headers[i] ?? string.Empty;
            
            // Replace empty header with generic name
            if (string.IsNullOrWhiteSpace(header))
                header = "Column";

            if (!counts.ContainsKey(header))
            {
                counts[header] = 0;
                result[i] = header;
            }
            else
            {
                counts[header]++;
                result[i] = $"{header}_{counts[header]}";
            }
        }

        return result;
    }

    /// <summary>
    /// Convert CSV field value to appropriate type
    /// </summary>
    private static object ConvertCsvValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Try parsing as JSON first (for complex data that was serialized)
        try
        {
            if ((value.StartsWith("{") && value.EndsWith("}")) ||
                (value.StartsWith("[") && value.EndsWith("]")))
            {
                using var jsonDoc = JsonDocument.Parse(value);
                return JsonSerializer.Deserialize<object>(value);
            }
        }
        catch
        {
            // Not valid JSON, continue with other types
        }

        // Try other primitive types
        if (bool.TryParse(value, out var boolVal)) 
            return boolVal;
            
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal)) 
            return intVal;
            
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var dblVal)) 
            return dblVal;

        // Just a regular string
        return value;
    }

    /// <summary>
    /// Parse JSON to list of records
    /// </summary>
    private static List<Dictionary<string, object>> ParseJsonToRecords(string jsonString)
    {
        var records = new List<Dictionary<string, object>>();
        
        try
        {
            using var jsonDoc = JsonDocument.Parse(jsonString);
            var root = jsonDoc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                // Already an array of records
                foreach (var element in root.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        var record = JsonElementToRecord(element);
                        records.Add(record);
                    }
                }
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                // Single object - wrap in an array
                var record = JsonElementToRecord(root);
                records.Add(record);
            }
            else
            {
                throw new ArgumentException("JSON must contain an object or array of objects");
            }
        }
        catch (JsonException ex)
        {
            throw new Exception($"JSON parsing error: {ex.Message}");
        }
        
        return records;
    }

    /// <summary>
    /// Convert JsonElement to a Dictionary
    /// </summary>
    private static Dictionary<string, object> JsonElementToRecord(JsonElement element)
    {
        var record = new Dictionary<string, object>();
        
        foreach (var property in element.EnumerateObject())
        {
            record[property.Name] = JsonValueToObject(property.Value);
        }
        
        return record;
    }

    /// <summary>
    /// Convert JsonElement value to appropriate object
    /// </summary>
    private static object JsonValueToObject(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                return JsonElementToRecord(element);
                
            case JsonValueKind.Array:
                var array = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    array.Add(JsonValueToObject(item));
                }
                return array;
                
            case JsonValueKind.String:
                return element.GetString();
                
            case JsonValueKind.Number:
                if (element.TryGetInt32(out int intValue))
                    return intValue;
                if (element.TryGetInt64(out long longValue))
                    return longValue;
                if (element.TryGetDouble(out double doubleValue))
                    return doubleValue;
                return element.GetDecimal();
                
            case JsonValueKind.True:
                return true;
                
            case JsonValueKind.False:
                return false;
                
            case JsonValueKind.Null:
                return null;
                
            default:
                return element.ToString();
        }
    }

    /// <summary>
    /// Generate CSV from records using CsvHelper
    /// </summary>
    private static string GenerateCsvFromRecords(List<Dictionary<string, object>> records)
    {
        if (records.Count == 0)
            return string.Empty;

        // Collect all unique keys across all records
        var allColumns = new HashSet<string>();
        foreach (var record in records)
        {
            foreach (var key in record.Keys)
            {
                allColumns.Add(key);
            }
        }

        // Sort columns alphabetically for consistent output
        var sortedColumns = allColumns.OrderBy(c => c).ToList();

        // Generate CSV
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            // Write header
            foreach (var column in sortedColumns)
            {
                csv.WriteField(column);
            }
            csv.NextRecord();

            // Write records
            foreach (var record in records)
            {
                foreach (var column in sortedColumns)
                {
                    if (record.TryGetValue(column, out var value))
                    {
                        // Handle different value types
                        if (value is Dictionary<string, object> dictValue || value is List<object> listValue)
                        {
                            // Serialize complex objects back to JSON
                            csv.WriteField(JsonSerializer.Serialize(value));
                        }
                        else if (value == null)
                        {
                            csv.WriteField(string.Empty);
                        }
                        else
                        {
                            csv.WriteField(value.ToString());
                        }
                    }
                    else
                    {
                        // Column not present in this record
                        csv.WriteField(string.Empty);
                    }
                }
                csv.NextRecord();
            }
        }

        return sb.ToString();
    }

    #endregion
}