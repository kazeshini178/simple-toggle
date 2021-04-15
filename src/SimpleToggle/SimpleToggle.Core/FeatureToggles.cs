using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleToggle.Core
{
    [Serializable]
    public class FeatureToggles : Dictionary<string, string>
    {
        public FeatureToggles() { }

        protected FeatureToggles(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
