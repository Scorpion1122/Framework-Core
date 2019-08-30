using System;

namespace Thijs.Core.Services
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ImplementsMonoServiceAttribute : Attribute
    {
        private Type interfaceType;
        public Type InterfaceType
        {
            get { return interfaceType; }
        }

        public ImplementsMonoServiceAttribute(Type interfaceType)
        {
            this.interfaceType = interfaceType;
        }
    }
}
