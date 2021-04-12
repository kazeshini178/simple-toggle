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

        public Task<bool> GetToggleValue(string toggleName)
        {
            if (!toggles.TryGetValue(toggleName, out var value))
            {
                return Task.FromResult(false);
            }

            _ = bool.TryParse(value, out var result);
            return Task.FromResult(result);
        }
    }
}
