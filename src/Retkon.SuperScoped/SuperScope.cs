using Retkon.SuperScoped.Exceptions;
using Retkon.SuperScoped.Internal;

namespace Retkon.SuperScoped;

public sealed record class SuperScope<TScope> where TScope : class, new()
{

    internal TScope Scope { get; }
    internal bool IsFixed { get; set; }

    public SuperScope(TScope scope)
    {
        this.Scope = scope;
    }

    public void WithScope(Action<TScope> scopingAction)
    {
        bool lockWasTaken = false;
        try
        {
            Monitor.Enter(typeof(SuperScopeLock<TScope>), ref lockWasTaken);

            if (this.IsFixed)
            {
                var newScope = new TScope();
                scopingAction(newScope);

                if (!this.Scope.Equals(newScope))
                    throw SuperScopeFixedException.FromType<TScope>();
            }
            else
            {
                scopingAction(this.Scope);
                this.IsFixed = true;
            }
        }
        finally
        {
            if (lockWasTaken)
            {
                Monitor.Exit(typeof(SuperScopeLock<TScope>));
            }
        }
    }

    public void ValidateScope()
    {
        if (!this.IsFixed)
            throw SuperScopeNotFixedException.FromType<TScope>();
    }

    public static implicit operator TScope(SuperScope<TScope> superScope)
    {
        if (!superScope.IsFixed)
            throw SuperScopeNotFixedException.FromType<TScope>();

        return superScope.Scope;
    }

}
