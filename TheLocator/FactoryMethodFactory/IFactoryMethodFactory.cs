using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyLocator.FactoryMethodFactory
{
    interface IFactoryMethodFactory
    {
        Func<object> CreateFactoryMethod(Type type);
    }
}
