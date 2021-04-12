using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleToggle.Core
{
    public interface IFeatureService
    { 
        Task<bool> GetToggleValue(string toggleName);
    }
}
