using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Options;
using SimpleToggle.Core;

namespace SimpleToggle.Sources.AWS
{
    public class ParameterStoreToggleSource : IToggleSource
    {
        private readonly FeatureToggles toggles;
        private readonly IAmazonSimpleSystemsManagement systemsManagement;

        public ParameterStoreToggleSource(IAmazonSimpleSystemsManagement systemsManagement, IOptions<FeatureToggles> toggles)
        {
            this.toggles = toggles.Value;
            this.systemsManagement = systemsManagement;
        }

        public async Task<bool> GetToggleValue(string toggleName)
        {
            if (!toggles.TryGetValue(toggleName, out var parameterName))
            {
                return false;
            }

            try
            {
                var response = await systemsManagement.GetParameterAsync(new GetParameterRequest() { Name = parameterName });
                if (!bool.TryParse(response.Parameter.Value, out bool value))
                {
                    return false;
                }

                return value;
            }
            catch (ParameterNotFoundException)
            {
                return false;
            }
        }
    }
}
