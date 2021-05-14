Adds AWS toggle sources for Simple Toggle. Includes ParameterStore, DynamoDb and Secrets Manager sources.

## Usage

1. Install from Nuget
```bash
dotnet add package SimpleToggle.Sources.AWS
# Optional: Add AWS SDK packages. ie:
dotnet add package AWSSDK.*
```

2. Update StartUp.cs
```csharp
public class StartUp
{
...
    public void ConfigureServices(IServiceCollection services)
    {
       ...
       services.AddAWSService<IAmazonSimpleSystemsManagement>(); // For ParameterStore
       services.AddFeatureService<ParameterStoreToggleSource>();
       ...
    }
...
}
```