using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Options;
using SimpleToggle.Core;

namespace SimpleToggle.Sources.AWS
{
    public class SecretsManagerToggleSource : IToggleSource
    {
        private readonly IAmazonSecretsManager secretsManager;
        private readonly FeatureToggles toggles;

        public SecretsManagerToggleSource(IAmazonSecretsManager secretsManager, IOptions<FeatureToggles> toggles)
        {
            this.secretsManager = secretsManager;
            this.toggles = toggles.Value;
        }

        public async Task<bool> GetToggleValue(string toggleName)
        {
            if (!toggles.TryGetValue(toggleName, out var parameterName))
            {
                return false;
            }

            try
            {
                var response = await secretsManager.GetSecretValueAsync(new GetSecretValueRequest() { SecretId = parameterName });
                if (!bool.TryParse(response.SecretString, out bool value))
                {
                    return false;
                }

                return value;
            }
            catch (AmazonSecretsManagerException e) when (e is InvalidParameterException || e is InvalidRequestException || e is ResourceNotFoundException)
            {
                return false;
            }
        }
    }
}
