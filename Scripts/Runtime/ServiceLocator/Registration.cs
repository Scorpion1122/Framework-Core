using System;

namespace TKO.Core.Services
{
    public class Registration : IDisposable
    {
        private readonly IServiceFactory factory;

        //Within paladin almost al services are used as a singleton, so it is true by default
        public bool AsSingleton { get; set; }
        private object singletonInstance;

        public Registration(IServiceFactory factory)
        {
            this.factory = factory;
            AsSingleton = false;
        }

        public Registration(object instance, IServiceFactory factory)
        {
            this.factory = factory;
            singletonInstance = instance;
            AsSingleton = true;
        }

        public object GetInstance(IServiceLocator source)
        {
            if (AsSingleton)
                return singletonInstance ?? (singletonInstance = factory.GetInstance(source));
            return factory.GetInstance(source);
        }

        public Registration SetAsSingleton(bool value)
        {
            AsSingleton = value;
            return this;
        }

        public void Dispose()
        {
            if (singletonInstance != null)
                factory.DisposeOfInstance(singletonInstance);
        }
    }
}
