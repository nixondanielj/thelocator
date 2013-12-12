using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyLocator
{
    public interface ILocator
    {
        void Register(Type abstrct, Type concrete);
        void Register(Type abstrct, Type concrete, bool shareable);
        void Register<T>(Type concrete);
        void Register<T>(Type concrete, bool shareable);
        void Register(IRegistry registry);
        T Locate<T>();
        object Locate(Type t);
    }
}
