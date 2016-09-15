using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSFApp.Common
{
    public class SettingsService
    {
        private ICodePackageActivationContext _serviceContext;
        public SettingsService()
        {
            _serviceContext = FabricRuntime.GetActivationContext();
        }

        public Dictionary<string, string> GetSection(string section)
        {
            var parameterDictionary = new Dictionary<string, string>();
            if (_serviceContext == null)
                return null;

            var configurationPackage = _serviceContext.GetConfigurationPackageObject("Config");
            if (configurationPackage != null)
            {
                var configSection = configurationPackage.Settings.Sections[section];
                if (configSection != null)
                {
                    foreach (var item in configSection.Parameters)
                    {
                        parameterDictionary.Add(item.Name, item.Value);
                    }
                }
            }
            return parameterDictionary;
        }
        public string GetSectionParameterValue(string section, string parameterKey)
        {
            try
            {
                if (_serviceContext == null)
                    return "";

                var parameterValue = "";
                var configurationPackage = _serviceContext.GetConfigurationPackageObject("Config");
                if (configurationPackage != null)
                {
                    var configSection = configurationPackage.Settings.Sections[section];
                    if (configSection != null)
                    {
                        var connectorParameter = configSection.Parameters[parameterKey];
                        if (connectorParameter != null)
                        {
                            parameterValue = connectorParameter.Value;
                        }
                    }
                }

                return parameterValue;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
