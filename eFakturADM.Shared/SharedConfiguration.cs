using System;
using System.Configuration;
using System.Xml;

namespace eFakturADM.Shared
{
    public class SharedConfiguration
    {
        private const string ConfigSectionName = "SharedConfigs";
        private const string ConfigSectionGroupName = "eFakturADM";
        public string AppLogger { get; set; }

        public static SharedConfiguration GetConfig()
        {
            return (SharedConfiguration)ConfigurationManager.GetSection(ConfigSectionGroupName + "/" + ConfigSectionName);
        }

        public void LoadConfigValues(XmlNode node)
        {
            XmlAttributeCollection attributeCollection = node.Attributes;
            if (attributeCollection == null)
                throw new ConfigurationErrorsException(ConfigSectionName + " - Node Attribute Collection is missing from configuration", node);

            if (attributeCollection["AppLogger"] == null)
                throw new ConfigurationErrorsException(ConfigSectionName + " - AppLogger is missing from configuration", node);

            AppLogger = attributeCollection["AppLogger"].Value;

        }
    }

    public class SharedConfigurationHelper : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new SharedConfiguration();
            config.LoadConfigValues(section);
            return config;
        }
    }
}
