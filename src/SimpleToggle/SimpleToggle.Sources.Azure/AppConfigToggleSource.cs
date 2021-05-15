using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<ToggleDetails>> GetAllToggles()
        {
            // Learn to work with this API to improve method
            //var request =  configurationClient.GetConfigurationSettingsAsync(new SettingSelector() { KeyFilter = SettingSelector.Any });

            var toggleKeys = toggles.Values.ToList();
            var toggleDetails = new List<ToggleDetails>();
            while (toggleKeys.Count != 0)
            {
                // SecretsManager API doesnt have a bulk retrieval according to the Docs
                // Consider a better approach, also consider error states
                var tasks = toggleKeys.Take(5)
                                     .Select(t => configurationClient.GetConfigurationSettingAsync(t))
                                     .ToList();

                var results = await Task.WhenAll(tasks);
                toggleDetails.AddRange(results.Select(r =>
                {
                    var setting = r.Value;
                    _ = bool.TryParse(setting.Value, out bool value);
                    return new ToggleDetails(setting.Key, value);
                }));
                toggleKeys.RemoveRange(0, tasks.Count);
            }

            return toggleDetails;
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

        public async Task UpdateToggleValue(string toggleName, bool value)
        {
            if (!toggles.TryGetValue(toggleName, out var parameterName))
            {
                return;
            }

            _ = await configurationClient.SetConfigurationSettingAsync(parameterName, value.ToString());
        }
    }
}
