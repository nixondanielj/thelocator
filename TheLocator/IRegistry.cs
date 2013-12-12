using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyLocator
{
    public interface IRegistry
    {
        Dictionary<Type, Type> GetRegistrations();
    }
}
