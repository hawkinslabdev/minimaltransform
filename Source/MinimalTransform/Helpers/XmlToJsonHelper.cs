using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Xml.Linq;

namespace MinimalTransform.Helpers;

// Helper for converting XML to JSON
public static class XmlToJsonHelper
{
    // Convert XML string to JSON string
    public static string ConvertXmlToJson(string xml, int indentation = 2)
    {
        try
        {
            // Parse XML
            XDocument doc = XDocument.Parse(xml);
            
            // Convert to JSON
            var jsonObject = ConvertXmlElementToJsonObject(doc.Root);
            
            // Serialize to JSON string with configurable indentation and proper character handling
            return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions 
            { 
                WriteIndented = indentation > 0,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Prevents unnecessary escaping of characters
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in XML to JSON conversion: {ex.Message}", ex);
        }
    }

    // Convert XElement to JSON-compatible object
    private static object ConvertXmlElementToJsonObject(XElement element)
    {
        if (element == null)
            return string.Empty;
            
        // Check if this element has no elements and only one text node
        if (!element.HasElements && element.Nodes().Count() <= 1)
        {
            // Return the text value or empty string if null
            return element.Value ?? string.Empty;
        }
        
        // Check if all child elements have the same name (potential array)
        var childElements = element.Elements().ToList();
        
        if (childElements.Count > 0)
        {
            // Group child elements by name to detect arrays
            var elementGroups = childElements.GroupBy(e => e.Name.LocalName).ToList();
            
            // If there's only one group and it has multiple elements, treat as an array
            if (elementGroups.Count == 1 && elementGroups[0].Count() > 1)
            {
                var array = new List<object>();
                foreach (var childElement in childElements)
                {
                    array.Add(ConvertXmlElementToJsonObject(childElement));
                }
                return array;
            }
            else
            {
                // Otherwise, treat as an object
                var obj = new Dictionary<string, object>();
                foreach (var group in elementGroups)
                {
                    string key = group.Key;
                    
                    if (group.Count() > 1)
                    {
                        // Multiple elements with same name = array property
                        var arrayProperty = new List<object>();
                        foreach (var item in group)
                        {
                            arrayProperty.Add(ConvertXmlElementToJsonObject(item));
                        }
                        obj[key] = arrayProperty;
                    }
                    else
                    {
                        // Single element = object property
                        obj[key] = ConvertXmlElementToJsonObject(group.First());
                    }
                }
                
                // Also process attributes if present
                foreach (var attr in element.Attributes())
                {
                    obj["@" + attr.Name.LocalName] = attr.Value;
                }
                
                return obj;
            }
        }
        else
        {
            // Element with no children but possible attributes
            if (element.Attributes().Any())
            {
                var obj = new Dictionary<string, object>();
                obj["#text"] = element.Value ?? string.Empty;
                
                foreach (var attr in element.Attributes())
                {
                    obj["@" + attr.Name.LocalName] = attr.Value;
                }
                return obj;
            }
            else
            {
                // Just a simple value
                return element.Value ?? string.Empty;
            }
        }
    }
}