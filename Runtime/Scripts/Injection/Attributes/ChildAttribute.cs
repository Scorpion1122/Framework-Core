using System;

namespace TKO.Core.Injection
{
    public class ChildAttribute : Attribute
    {
        private Type injectType = null;

        public Type InjectType { get { return injectType; } }

        public ChildAttribute()
        {
        }

        public ChildAttribute(Type injectType)
        {
            this.injectType = injectType;
        }
    }
}