using System;

namespace Thijs.Core.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ServiceAttribute : Attribute
    {
        private object id = null;
        private Type injectType = null;

        public object Id { get { return id; } }
        public Type InjectType { get { return injectType; } }

        public ServiceAttribute()
        {
        }

        public ServiceAttribute(Type injectType)
        {
            this.injectType = injectType;
        }

        public ServiceAttribute(string id)
        {
            this.id = id;
        }

        public ServiceAttribute(string id, Type injectType)
        {
            this.injectType = injectType;
            this.id = id;
        }
    }
}