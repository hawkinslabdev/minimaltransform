using System;

namespace MinimalTransform.Helpers;

// Helper for converting XML to YAML (via JSON)
public static class XmlToYamlHelper
{
    // Convert XML string to YAML string
    public static string ConvertXmlToYaml(string xmlString)
    {
        try
        {
            if (!CommonHelper.IsValidInput(xmlString))
                throw new ArgumentException("Invalid XML data");
            
            // Convert XML to JSON first (without indentation for intermediate format)
            string jsonString = XmlToJsonHelper.ConvertXmlToJson(xmlString, 0);
            
            // Convert JSON to YAML
            string yaml = JsonToYamlHelper.ConvertJsonToYaml(jsonString);
            
            return yaml;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting XML to YAML: {ex.Message}", ex);
        }
    }
}