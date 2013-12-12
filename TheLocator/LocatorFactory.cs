using DependencyLocator.FactoryMethodFactory;
using DependencyLocator.FactoryMethodFactory.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyLocator
{
    public static class LocatorFactory
    {
        private static ILocator locator;

        public static ILocator GetLocator()
        {
            if (locator == null)
            {
                locator = new DefaultLocator(new DefaultFactoryMethodFactory());
                RegisterAbstractions(locator);
            }
            return locator;
        }

        private static void RegisterAbstractions(ILocator locator)
        {
            locator.Register<IFactoryMethodFactory>(typeof(DefaultFactoryMethodFactory), true);
        }
    }
}
