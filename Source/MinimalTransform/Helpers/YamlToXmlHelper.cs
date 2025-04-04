using System;
using System.Text;
using System.Xml;

namespace MinimalTransform.Helpers;

// Helper for converting YAML to XML (via JSON)
public static class YamlToXmlHelper
{
    // Convert YAML string to XML string
    public static string ConvertYamlToXml(string yamlString, int indentation = 2, string rootName = "root")
    {
        try
        {
            if (!CommonHelper.IsValidInput(yamlString))
                throw new ArgumentException("Invalid YAML data");
            
            // Convert YAML to JSON first (without indentation for intermediate format)
            string jsonString = YamlToJsonHelper.ConvertYamlToJson(yamlString, 0);
            
            // Convert JSON to XML
            var node = JsonToXmlHelper.ConvertJsonToXml(jsonString, rootName);
            
            // Format the XML with configurable indentation
            var settings = JsonToXmlHelper.GetXmlSettings(indentation);
            
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                node.WriteTo(writer);
            }
            
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting YAML to XML: {ex.Message}", ex);
        }
    }
}