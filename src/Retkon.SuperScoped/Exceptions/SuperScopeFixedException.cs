using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.SuperScoped.Exceptions;
public class SuperScopeFixedException : InvalidOperationException
{

    internal SuperScopeFixedException(Type scopeType)
        : base($"A SuperScope of type 'SuperScope<{scopeType.FullName}>' has already been Fixed to a different set of values.")
    {
    }

    internal static SuperScopeFixedException FromType<TScope>() where TScope : class => new(typeof(TScope));

}
