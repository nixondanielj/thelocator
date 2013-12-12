using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyLocator.FactoryMethodFactory.Concrete
{
    internal class DefaultFactoryMethodFactory : IFactoryMethodFactory
    {
        public Func<object> CreateFactoryMethod(Type type)
        {
            var ctorParams = type.GetConstructors()[0].GetParameters();
            object[] values = new object[ctorParams.Length];
            Func<object> factoryMethod = (() =>
                {
                    for (int i = 0; i < ctorParams.Length; i++)
                    {
                        values[i] = LocatorFactory.GetLocator().Locate(ctorParams[i].ParameterType);
                    }
                    return Activator.CreateInstance(type, values);
                });
            return factoryMethod;
        }

        public Func<T> CreateFactoryMethod<T>()
        {
            return (Func<T>)(()=> (T) CreateFactoryMethod(typeof(T))());
        }
    }
}
