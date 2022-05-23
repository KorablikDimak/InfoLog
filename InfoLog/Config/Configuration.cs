using System.Collections.Generic;
using System.Xml.Linq;

namespace InfoLog.Config;

/// <summary>
/// 
/// </summary>
public class Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public List<Dictionary<string, string>> Configs { get; }

    /// <summary>
    /// Create dictionary {attribute:param} from xml file.
    /// </summary>
    /// <param name="xmlConfigPath">Absolute or relative path to .xml file</param>
    public Configuration(string xmlConfigPath)
    {
        var xmlDocument = XDocument.Load(xmlConfigPath);
        var xmlLogger = xmlDocument.Element("logger");
        var targets = xmlLogger?.Element("targets");
        if (targets == null) return;
            
        Configs = new List<Dictionary<string, string>>();
        foreach (var target in targets.Elements("target"))
        {
            var config = new Dictionary<string, string>();
            foreach (var attribute in target.Attributes())
            {
                config[attribute.Name.LocalName] = attribute.Value;
            }

            Configs.Add(config);
        }
    }
}