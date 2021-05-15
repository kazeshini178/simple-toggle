# Simple Toggle

Simple feature toggling library to work with basic on/off toggles, allowing the toggles to be stored in the service of your choice.

[![.NET](https://github.com/kazeshini178/simple-toggle/actions/workflows/dotnetCI.yml/badge.svg?branch=main)](https://github.com/kazeshini178/simple-toggle/actions/workflows/dotnetCI.yml)


## Toggle Sources

* In Memory
* AWS
  * Parameter Store
  * DynamoDB Table
  * Secrets Manager
* Azure 
  * App Configuration


### Usage

1. Install from Nuget
```bash
dotnet add package SimpleToggle.Core
```

2. Update StartUp.cs
```csharp
public class StartUp
{
...
    public void ConfigureServices(IServiceCollection services)
    {
       ...
       services.AddFeatureService<InMemoryToggleSource>();
       ...
    }
...
}
```

## TODO Lists

* [x] Add Additional AWS Sources
* [ ] Add GPC and Azure Equivalents 
* [ ] Allow for some basic configuration options
* [x] Publish to Nuget
* [x] Added tests

## Alternatives

For more feature rich alternatives, that allow A/B testing toggles, more then just on/off toggle and user targeting and tracking. 

* [LaunchDarkly](https://launchdarkly.com/)
* [Optimizely](https://www.optimizely.com/) 
* [Split: Feature Flags](https://www.split.io/product/feature-flags/) 
* [Feature Management .Net](https://github.com/microsoft/FeatureManagement-Dotnet) - Azure 

