using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleToggle.Core
{
    public interface IFeatureService
    {
        Task<bool> GetToggleValue(string toggleName);
        Task<List<ToggleDetails>> GetAllToggles();
        Task UpdateToggleValue(string toggleName, bool value);
        Task UpdateToggleValue(ToggleDetails toggle);
    }
}
