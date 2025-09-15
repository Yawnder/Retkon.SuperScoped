using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Retkon.SuperScoped.Internal;
public interface ISuperScopeProvider
{
    SuperScope<TScope> GetOrCreate<TScope>() where TScope : class, new();
}

internal class SuperScopeProvider : ISuperScopeProvider
{

    private Dictionary<Type, object> superScopes = [];

    public SuperScope<TScope> GetOrCreate<TScope>()
        where TScope : class, new()
    {
        if (!this.superScopes.TryGetValue(typeof(TScope), out var superScope))
        {
            lock (this.superScopes)
            {
                if (!this.superScopes.TryGetValue(typeof(TScope), out superScope))
                {
                    superScope = new SuperScope<TScope>(new());
                    this.superScopes.Add(typeof(TScope), superScope);
                }
            }
        }

        return (SuperScope<TScope>)superScope;
    }

}
