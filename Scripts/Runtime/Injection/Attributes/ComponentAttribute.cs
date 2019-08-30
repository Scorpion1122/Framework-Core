using System;

namespace Thijs.Core.Injection
{
    public class ComponentAttribute : Attribute
    {
        private Type injectType = null;

        public Type InjectType { get { return injectType; } }

        public ComponentAttribute()
        {
        }

        public ComponentAttribute(Type injectType)
        {
            this.injectType = injectType;
        }
    }
}