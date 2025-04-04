using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace MinimalTransform.Helpers;

// Common helper methods shared across format converters
public static class CommonHelper
{
    // Default indentation for formatted output
    public const int DefaultIndentation = 2;
    
    // Get JsonSerializerOptions with appropriate settings
    public static JsonSerializerOptions GetJsonSerializerOptions(int indentation = DefaultIndentation, bool relaxedEncoding = true)
    {
        var options = new JsonSerializerOptions
        { 
            WriteIndented = indentation > 0,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        
        if (relaxedEncoding)
        {
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }
        
        return options;
    }
    
    // Get XML writer settings with specified indentation
    public static XmlWriterSettings GetXmlSettings(int indentation = DefaultIndentation)
    {
        return new XmlWriterSettings 
        { 
            Indent = indentation > 0,
            IndentChars = new string(' ', indentation),
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };
    }
    
    // Get appropriate content type for the given format
    public static string GetContentType(string format)
    {
        return format.ToLower() switch
        {
            "xml" => "application/xml; charset=utf-8",
            "json" => "application/json; charset=utf-8",
            "yaml" => "application/x-yaml; charset=utf-8",
            "csv" => "text/csv; charset=utf-8",
            _ => "text/plain; charset=utf-8"
        };
    }
    
    // Validate that a string is not null or whitespace
    public static bool IsValidInput(string input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }
}