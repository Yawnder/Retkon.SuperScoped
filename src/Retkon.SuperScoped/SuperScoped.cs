using Microsoft.Extensions.DependencyInjection;
using Retkon.SuperScoped.Internal;

namespace Retkon.SuperScoped;

public sealed class SuperScoped<TInstance> where TInstance : class
{
    private readonly IServiceProvider serviceProvider;
    private readonly ISuperScopeProvider superScopeProvider;

    public SuperScoped(
        IServiceProvider serviceProvider,
        ISuperScopeProvider superScopeProvider)
    {
        this.serviceProvider = serviceProvider;
        this.superScopeProvider = superScopeProvider;
    }

    public TInstance SuperScope<TScope>(Action<TScope> scopingAction)
        where TScope : class, new()
    {
        var superScope = this.superScopeProvider.GetOrCreate<TScope>();
        superScope.WithScope(scopingAction);

        var superScopedInstance = this.serviceProvider.GetRequiredService<TInstance>();
        return superScopedInstance;
    }

    public TInstance SuperScope<TScope>()
        where TScope : class, new()
    {
        var superScope = this.serviceProvider.GetRequiredService<SuperScope<TScope>>();
        superScope.ValidateScope();

        var superScopedInstance = this.serviceProvider.GetRequiredService<TInstance>();
        return superScopedInstance;
    }

}
