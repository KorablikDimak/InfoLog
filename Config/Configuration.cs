using System.Collections.Generic;
using System.Xml.Linq;

namespace InfoLog.Config
{
    public class Configuration
    {
        public List<Dictionary<string, string>> Configs { get; }

        public Configuration(string xmlConfigPath)
        {
            var xmlDocument = XDocument.Load(xmlConfigPath);
            var xmlLogger = xmlDocument.Element("logger");
            if (xmlLogger == null) return;
            var targets = xmlLogger.Element("targets");
            if (targets == null) return;
            
            Configs = new List<Dictionary<string, string>>();
            foreach (var target in targets.Elements("target"))
            {
                Dictionary<string, string> config = new Dictionary<string, string>();
                foreach (var attribute in target.Attributes())
                {
                    config[attribute.Name.LocalName] = attribute.Value;
                }

                Configs.Add(config);
            }
        }
    }
}