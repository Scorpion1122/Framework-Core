using System;

namespace Thijs.Core.Services
{
    public abstract class BaseServiceFactory : IServiceFactory
    {
        protected Type instanceType;
        private object solvingInstance;

        protected BaseServiceFactory(Type instanceType)
        {
            this.instanceType = instanceType;
        }

        public object GetInstance(IServiceLocator source)
        {
            if (solvingInstance != null)
                return solvingInstance; //This situation arises with circular dependencies

            solvingInstance = CreateInstanceObject();
            if (ServiceInjector.HasInjectableAttribute(instanceType))
                ServiceInjector.InjectInto(source, solvingInstance);

            object instance = solvingInstance;
            solvingInstance = null;
            return instance;
        }

        protected abstract object CreateInstanceObject();
        public abstract void DisposeOfInstance(object instance);
    }

}
