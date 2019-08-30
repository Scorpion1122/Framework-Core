using System;

namespace Thijs.Core.Services
{
    public class ClassFactory : BaseServiceFactory
    {
        public ClassFactory(Type classType) : base(classType) { }

        protected override object CreateInstanceObject()
        {
            return Activator.CreateInstance(instanceType);
        }

        public override void DisposeOfInstance(object instance)
        {
            if (instance is IDisposable)
                ((IDisposable)instance).Dispose();
        }
    }
}
