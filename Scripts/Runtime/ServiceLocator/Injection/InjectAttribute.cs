using System;

namespace Thijs.Core.Services
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        private object id = null;
        private Type injectType = null;

        public object Id { get { return id; } }
        public Type InjectType { get { return injectType; } }

        public InjectAttribute()
        {
        }

        public InjectAttribute(Type injectType)
        {
            this.injectType = injectType;
        }

        public InjectAttribute(string id)
        {
            this.id = id;
        }

        public InjectAttribute(string id, Type injectType)
        {
            this.injectType = injectType;
            this.id = id;
        }
    }
}
