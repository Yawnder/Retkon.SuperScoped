using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Retkon.SuperScoped.Internal;

namespace Retkon.SuperScoped;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSuperScope<TScope>(this IServiceCollection serviceCollection) where TScope : class, new()
    {
        serviceCollection.TryAddScoped<ISuperScopeProvider, SuperScopeProvider>();

        serviceCollection.AddScoped<TScope>(sp =>
        {
            var superScopeProvider = sp.GetRequiredService<ISuperScopeProvider>();
            return superScopeProvider.GetOrCreate<TScope>();
        });

        serviceCollection.AddScoped<SuperScope<TScope>>(sp =>
        {
            var superScopeProvider = sp.GetRequiredService<ISuperScopeProvider>();
            return superScopeProvider.GetOrCreate<TScope>();
        });

        serviceCollection.AddScoped(typeof(SuperScoped<>));

        return serviceCollection;
    }
}
