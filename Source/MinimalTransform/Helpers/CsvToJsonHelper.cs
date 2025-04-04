using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

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

            var records = new List<ExpandoObject>();

            using (var reader = new StringReader(csvString))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null
            }))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    dynamic record = new ExpandoObject();
                    var recordDict = (IDictionary<string, object>)record;

                    foreach (var header in csv.HeaderRecord ?? Array.Empty<string>())
                    {
                        var rawValue = csv.GetField(header);
                        recordDict[header] = TryParseJsonOrPrimitive(rawValue);
                    }

                    records.Add(record);
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = indentation > 0,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(records, jsonOptions);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting CSV to JSON: {ex.Message}", ex);
        }
    }

    // Attempt to infer the most accurate type from a CSV field
    private static object? TryParseJsonOrPrimitive(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Try parse as JSON array or object
        try
        {
            using var jsonDoc = JsonDocument.Parse(value);
            return jsonDoc.RootElement.Clone(); // Preserve structure
        }
        catch
        {
            // Fallback: try parsing as bool, int, double
            if (bool.TryParse(value, out var boolVal)) return boolVal;
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal)) return intVal;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var dblVal)) return dblVal;

            return value; // fallback to string
        }
    }
}

