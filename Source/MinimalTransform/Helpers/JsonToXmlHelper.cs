using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace MinimalTransform.Helpers;

// Helper for converting JSON to XML
public static class JsonToXmlHelper
{
    // Convert JSON string to XML node
    public static XNode ConvertJsonToXml(string json, string rootName = "root")
    {
        try
        {
            // Parse JSON using System.Text.Json
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                return CreateXmlFromJsonElement(doc.RootElement, rootName);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in JSON to XML conversion: {ex.Message}", ex);
        }
    }

    // Create XML element from JsonElement
    private static XNode CreateXmlFromJsonElement(JsonElement element, string elementName)
    {
        // Handle element name (replace invalid XML characters)
        elementName = GetSafeElementName(elementName);
        
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                XElement objectElement = new XElement(elementName);
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    var childElement = CreateXmlFromJsonElement(property.Value, property.Name);
                    if (childElement != null)
                    {
                        objectElement.Add(childElement);
                    }
                }
                return objectElement;

            case JsonValueKind.Array:
                // Create parent element with child items
                XElement arrayElement = new XElement(elementName);
                
                foreach (JsonElement item in element.EnumerateArray())
                {
                    // Determine appropriate array item name
                    string itemName = DetermineArrayItemName(item, elementName);
                    arrayElement.Add(CreateXmlFromJsonElement(item, itemName));
                }
                return arrayElement;

            case JsonValueKind.String:
                return new XElement(elementName, element.GetString());

            case JsonValueKind.Number:
                return new XElement(elementName, element.GetRawText());

            case JsonValueKind.True:
                return new XElement(elementName, "true");

            case JsonValueKind.False:
                return new XElement(elementName, "false");

            case JsonValueKind.Null:
                return new XElement(elementName); // Empty element for null values

            default:
                throw new InvalidOperationException($"Unsupported JSON value kind: {element.ValueKind}");
        }
    }

    // Determine appropriate name for array item elements
    private static string DetermineArrayItemName(JsonElement item, string parentName)
    {
        // Singularize the parent name if it ends with 's' (common convention)
        if (parentName.EndsWith("s", StringComparison.OrdinalIgnoreCase) && parentName.Length > 1)
        {
            return parentName.Substring(0, parentName.Length - 1);
        }
        
        // If the item is an object, try to find a "type" or "name" property
        if (item.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in new[] { "type", "name", "id", "key" })
            {
                if (item.TryGetProperty(prop, out var typeProp) && typeProp.ValueKind == JsonValueKind.String)
                {
                    string propValue = typeProp.GetString();
                    if (!string.IsNullOrEmpty(propValue) && IsValidXmlName(propValue))
                    {
                        return propValue;
                    }
                }
            }
        }
        
        // Default fallback
        return "item";
    }

    // Ensure XML element names are valid
    private static string GetSafeElementName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "element";
            
        // Replace invalid characters
        var validName = new string(name.Select(c => IsValidXmlNameChar(c) ? c : '_').ToArray());
        
        // Ensure name starts with a letter or underscore
        if (!IsValidXmlNameStartChar(validName[0]))
            validName = "_" + validName;
            
        return validName;
    }

    // Check if string is a valid XML element name
    private static bool IsValidXmlName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
            
        return IsValidXmlNameStartChar(name[0]) && name.Skip(1).All(IsValidXmlNameChar);
    }

    // Check if character is valid as first character of XML element name
    private static bool IsValidXmlNameStartChar(char c)
    {
        return char.IsLetter(c) || c == '_';
    }

    // Check if character is valid in XML element name
    private static bool IsValidXmlNameChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_';
    }

    // Get XML writer settings with specified indentation
    public static XmlWriterSettings GetXmlSettings(int indentation = 2)
    {
        return new XmlWriterSettings 
        { 
            Indent = indentation > 0,
            IndentChars = new string(' ', indentation),
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };
    }
}