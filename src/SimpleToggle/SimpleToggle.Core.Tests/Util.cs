using System;
using Microsoft.Extensions.Logging;

namespace SimpleToggle.Core.Tests
{
    public class Util
    {
        public static ILogger<T> CreateLogger<T>() => LoggerFactory.Create(builder => { }).CreateLogger<T>();
    }
}
