using System;

namespace Thijs.Core.Injection
{
    public class ParentAttribute : Attribute
    {
        private Type injectType = null;

        public Type InjectType { get { return injectType; } }

        public ParentAttribute()
        {
        }

        public ParentAttribute(Type injectType)
        {
            this.injectType = injectType;
        }
    }
}