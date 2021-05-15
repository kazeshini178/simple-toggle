using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SimpleToggle.Core;

namespace SimpleToggle.Sources
{
    public class InMemoryToggleSource : IToggleSource
    {
        private readonly FeatureToggles toggles;

        public InMemoryToggleSource(IOptions<FeatureToggles> toggles)
        {
            this.toggles = toggles.Value;
        }

        public Task<List<ToggleDetails>> GetAllToggles()
        {
            var toggleDetails = toggles.Select(t =>
            {
                _ = bool.TryParse(t.Value, out var result);
                return new ToggleDetails(t.Key, result);
            }).ToList();

            return Task.FromResult(toggleDetails);
        }

        public Task<bool> GetToggleValue(string toggleName)
        {
            if (!toggles.TryGetValue(toggleName, out var value))
            {
                return Task.FromResult(false);
            }

            _ = bool.TryParse(value, out var result);
            return Task.FromResult(result);
        }

        public Task UpdateToggleValue(string toggleName, bool value)
        {
            toggles[toggleName] = value.ToString();
            return Task.CompletedTask;
        }
    }
}
