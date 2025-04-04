using System;
using System.Xml;
using System.Text.Json;
using System.Collections.Generic;

namespace MinimalTransform.Helpers
{
    public static class XmlToCsvHelper
    {
        public static string ConvertXmlToCsv(string xmlString)
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
    }
}
