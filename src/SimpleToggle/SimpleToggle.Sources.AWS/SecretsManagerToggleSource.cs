using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<ToggleDetails>> GetAllToggles()
        {
            var secretIds = toggles.Values.ToList();
            var toggleDetails = new List<ToggleDetails>();
            while (secretIds.Count != 0)
            {
                // SecretsManager API doesnt have a bulk retrieval according to the Docs
                // Consider a better approach, also consider error states
                var tasks = secretIds.Take(5)
                                     .Select(t => secretsManager.GetSecretValueAsync(new GetSecretValueRequest() { SecretId = t }))
                                     .ToList();

                var results = await Task.WhenAll(tasks);
                toggleDetails.AddRange(results.Select(r =>
                {
                    _ = bool.TryParse(r.SecretString, out bool value);
                    return new ToggleDetails(r.Name, value);
                }));
                secretIds.RemoveRange(0, tasks.Count);
            }

            return toggleDetails;
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

        public async Task UpdateToggleValue(string toggleName, bool value)
        {
            if (!toggles.TryGetValue(toggleName, out var parameterName))
            {
                return;
            }

            _ = await secretsManager.PutSecretValueAsync(new PutSecretValueRequest()
            {
                SecretId = parameterName,
                SecretString = value.ToString()
            });
        }
    }
}
