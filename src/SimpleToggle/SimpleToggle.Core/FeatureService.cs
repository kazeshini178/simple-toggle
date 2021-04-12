using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleToggle.Core
{
    public class FeatureService<TSource> : IFeatureService where TSource : IToggleSource
    {
        private readonly Dictionary<string, bool> cachedValues = new Dictionary<string, bool>();

        private readonly TSource source;
        private readonly ILogger logger;

        public FeatureService(TSource source, ILogger<FeatureService<TSource>> logger)
        {
            this.source = source;
            this.logger = logger;
        }

        public async Task<bool> GetToggleValue(string toggleName)
        {
            logger.LogDebug("Confirming if Toggle value has been cached");
            if (cachedValues.ContainsKey(toggleName))
            {
                return cachedValues[toggleName];
            }

            logger.LogDebug("Retrieving value from Source");
            var value = await source.GetToggleValue(toggleName);
            cachedValues.Add(toggleName, value);
            return value;
        }
    }
}
