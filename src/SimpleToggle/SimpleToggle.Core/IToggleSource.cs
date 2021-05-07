using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleToggle.Core
{
    public interface IToggleSource
    {
        Task<bool> GetToggleValue(string toggleName);
        Task<List<ToggleDetails>> GetAllToggles();
        Task UpdateToggleValue(string toggleName, bool value);
    }
}
