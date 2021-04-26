using System;
using System.Threading.Tasks;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Options;
using SimpleToggle.Core;

namespace SimpleToggle.Sources.Azure
{
    public class AppConfigToggleSource : IToggleSource
    {
        private readonly FeatureToggles toggles;
        private readonly ConfigurationClient configurationClient;

        public AppConfigToggleSource(ConfigurationClient configurationClient, IOptions<FeatureToggles> toggles)
        {
            this.configurationClient = configurationClient;
            this.toggles = toggles.Value;
        }

        public async Task<bool> GetToggleValue(string toggleName)
        {
            if (!toggles.TryGetValue(toggleName, out var parameterName))
            {
                return false;
            }

            var response = await configurationClient.GetConfigurationSettingAsync(parameterName);
            // Need to confirm 
            if (!bool.TryParse(response?.Value?.Value, out bool value))
            {
                return false;
            }

            return value;

        }
    }
}
