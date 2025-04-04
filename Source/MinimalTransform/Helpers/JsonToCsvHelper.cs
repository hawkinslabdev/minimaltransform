using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace MinimalTransform.Helpers;

// Helper for converting JSON to CSV
public static class JsonToCsvHelper
{
    // Convert JSON string to CSV string
    public static string ConvertJsonToCsv(string jsonString)
    {
        try
        {
            // Parse JSON to array of objects
            object[] items;

            try
            {
                // Try to parse as array
                items = JsonSerializer.Deserialize<object[]>(jsonString);
            }
            catch
            {
                // If not an array, wrap single object in array
                var singleObject = JsonSerializer.Deserialize<object>(jsonString);
                items = new[] { singleObject };
            }

            if (items == null || items.Length == 0)
                throw new ArgumentException("JSON data doesn't contain any items");

            // Convert to CSV
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                // Extract properties from first object to determine headers
                var firstItem = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    JsonSerializer.Serialize(items[0]));

                if (firstItem == null)
                    throw new ArgumentException("Could not determine CSV structure from JSON");

                // Write headers
                foreach (var key in firstItem.Keys)
                {
                    csv.WriteField(key);
                }
                csv.NextRecord();

                // Write data
                foreach (var item in items)
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        JsonSerializer.Serialize(item));

                    if (dict != null)
                    {
                        foreach (var key in firstItem.Keys)
                        {
                            if (dict.TryGetValue(key, out var value))
                            {
                                if (value is JsonElement element)
                                {
                                    switch (element.ValueKind)
                                    {
                                        case JsonValueKind.Object:
                                        case JsonValueKind.Array:
                                            csv.WriteField(JsonSerializer.Serialize(element));
                                            break;
                                        case JsonValueKind.String:
                                        case JsonValueKind.Number:
                                        case JsonValueKind.True:
                                        case JsonValueKind.False:
                                            csv.WriteField(element.ToString());
                                            break;
                                        default:
                                            csv.WriteField("");
                                            break;
                                    }
                                }
                                else
                                {
                                    csv.WriteField(JsonSerializer.Serialize(value));
                                }
                            }
                            else
                            {
                                csv.WriteField("");
                            }
                        }
                        csv.NextRecord();
                    }
                }
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting JSON to CSV: {ex.Message}", ex);
        }
    }
}
