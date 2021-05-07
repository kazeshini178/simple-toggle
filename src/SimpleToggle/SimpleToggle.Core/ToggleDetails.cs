using System;
namespace SimpleToggle.Core
{
    public class ToggleDetails
    {
        public ToggleDetails(string name, bool value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public bool Value { get; }
    }
}
