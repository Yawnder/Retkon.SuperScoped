using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.SuperScoped.Internal;
internal class SuperScopeLock<T> where T : class
{
    public SuperScopeLock() => throw new InvalidOperationException();
}