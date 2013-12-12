using DependencyLocator.FactoryMethodFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityAbstractions;

namespace DependencyLocator
{
    public class DefaultLocator : ILocator
    {
        private Dictionary<Type, Lazy<object>> ShareableInstanceRegistry { get; set; }

        // making this lazy may have been overkill, but I enjoyed it
        private Dictionary<Type, Lazy<Func<object>>> FactoryMethodRegistry { get; set; }

        private ILogger logger { get; set; }

        private IFactoryMethodFactory factoryFactory { get; set; }

        internal DefaultLocator(IFactoryMethodFactory factory)
        {
            ShareableInstanceRegistry = new Dictionary<Type, Lazy<object>>();
            FactoryMethodRegistry = new Dictionary<Type, Lazy<Func<object>>>();
            this.factoryFactory = factory;
        }

        public void Register(Type abstrct, Type concrete)
        {
            // remove any existing entries
            ClearType(abstrct);
            // register the lazily created factory method for this item
            FactoryMethodRegistry.Add(abstrct, new Lazy<Func<object>>(()=>this.factoryFactory.CreateFactoryMethod(concrete)));
        }

        public void Register(Type abstrct, Type concrete, bool shareable)
        {
            // registers the factory method
            Register(abstrct, concrete);
            if(shareable)
            {
                ShareableInstanceRegistry.Add(abstrct, 
                    new Lazy<object>(()=>this.FactoryMethodRegistry[abstrct].Value())
                    );
            }
        }

        public void Register<T>(Type concrete)
        {
            Register(typeof(T), concrete);
        }

        public void Register<T>(Type concrete, bool shareable)
        {
            Register(typeof(T), concrete, shareable);
        }

        public T Locate<T>()
        {
            return (T)Locate(typeof(T));
        }

        public object Locate(Type t)
        {
            return Locate(t, false);
        }

        private object Locate(Type t, bool triedMagic)
        {
            object instance = null;
            // if a shared instance (or a process for creating one) is already registered
            if (ShareableInstanceRegistry.ContainsKey(t))
            {
                instance = ShareableInstanceRegistry[t].Value;
            }
            // else if we know how to create one
            else if (FactoryMethodRegistry.ContainsKey(t))
            {
                instance = FactoryMethodRegistry[t].Value();
            }
            // else, we must not know about this thing - have we tried guessing how to build it?
            else if(!triedMagic)
            {
                logger.Warn("Nothing registered for {0}, attempting black magic...", t);
                instance = AttemptBlindConstruction(t);
            }
            return instance;
        }

        private object AttemptBlindConstruction(Type t)
        {
            // try creating one
            object instance = null;
            try
            {
                logger.Warn("Attempting default registration...");
                this.Register(t, t);
                logger.Warn("Attempting creation...");
                this.Locate(t, true);
                logger.Warn("Creation appears successful...");
                logger.Info("Creation will be reattempted in the same manner next time.");
            }
            catch (Exception e)
            {
                logger.Severe("Couldn't come up with an instance for {0}, returning default", t);
                logger.Error(e, "Error returned during instantiation attempt:");
                ClearType(t);
                logger.Info("Rolled back registration");
            }
            return instance;
        }

        private void ClearType(Type abstrct)
        {
            FactoryMethodRegistry.Remove(abstrct);
            ShareableInstanceRegistry.Remove(abstrct);
        }


        public void Register(IRegistry registry)
        {
            foreach(var t in registry.GetRegistrations())
            {
                this.Register(t.Key, t.Value);
            }
        }
    }
}
