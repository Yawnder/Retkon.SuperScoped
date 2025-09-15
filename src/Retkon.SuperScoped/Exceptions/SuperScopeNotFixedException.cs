using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.SuperScoped.Exceptions;
public class SuperScopeNotFixedException : InvalidOperationException
{

    internal SuperScopeNotFixedException(Type scopeType)
        : base($"A SuperScope of type 'SuperScope<{scopeType.FullName}>' has to be Fixed before being used.")
    {
    }

    internal static SuperScopeNotFixedException FromType<TScope>() where TScope : class => new(typeof(TScope));

}
