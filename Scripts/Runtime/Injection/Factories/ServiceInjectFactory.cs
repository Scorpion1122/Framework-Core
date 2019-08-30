using System;
using System.Collections.Generic;
using System.Reflection;
using Thijs.Core.Services;

namespace Thijs.Core.Injection
{
    public class ServiceInjectFactory : InjectFactory<ServiceAttribute>
    {
        public override bool AtEditTime => false;
        public override bool MustBeMonobehaviour { get { return false; } }

        protected override void InjectInto(Type type, InjectDefinition<ServiceAttribute> definition, object target)
        {
            foreach (KeyValuePair<FieldInfo, ServiceAttribute> pair in definition.Fields)
            {
                Type injectType = pair.Value.InjectType ?? pair.Key.FieldType;
                object id = pair.Value.Id ?? ServiceLocator.DEFAULT_SERVICE_KEY;
                object instance = ServiceLocator.Instance.GetInstance(injectType, id);
                pair.Key.SetValue(target, instance);
            }
        }
    }
}