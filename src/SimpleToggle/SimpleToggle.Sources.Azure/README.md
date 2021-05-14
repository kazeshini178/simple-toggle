Adds Azure toggle sources for Simple Toggle. Includes App Config source.

## Usage

1. Install from Nuget
```bash
dotnet add package SimpleToggle.Sources.Azure
# Optional: Add Azure AppConfig packages. ie:
dotnet add package Azure.Data.AppConfiguration
```

2. Update StartUp.cs
```csharp
public class StartUp
{
...
    public void ConfigureServices(IServiceCollection services)
    {
       ...
       // Follow this [Guide](https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/appconfiguration/Azure.Data.AppConfiguration/README.md) to setup AppConfig
       services.AddFeatureService<AppConfigToggleSource>();
       ...
    }
...
}
```