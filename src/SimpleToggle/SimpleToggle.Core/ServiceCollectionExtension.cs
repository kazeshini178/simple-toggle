using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleToggle.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSimpleFeatureToggle<TSource>(this IServiceCollection services) where TSource : IToggleSource
        {
            services.AddScoped<IFeatureService, FeatureService<TSource>>();
            // NOTE: Would prefer - services.AddScoped<IToggleSource, TSource>();
            services.AddScoped(typeof(IToggleSource), typeof(TSource));
            return services;
        }
    }
}
