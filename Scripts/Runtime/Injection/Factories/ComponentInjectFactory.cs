using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Thijs.Core.Injection
{
    public class ComponentInjectFactory : InjectFactory<ComponentAttribute>
    {
        public override bool AtEditTime => true;
        public override bool MustBeMonobehaviour { get { return true; } }
        
        protected override void InjectInto(Type type, InjectDefinition<ComponentAttribute> definition, object target)
        {
            GameObject gameObject = ((MonoBehaviour)target).gameObject;
            foreach (KeyValuePair<FieldInfo, ComponentAttribute> pair in definition.Fields)
            {
                Type injectType = pair.Value.InjectType ?? pair.Key.FieldType;
                Component instance = gameObject.GetComponent(injectType);
                pair.Key.SetValue(target, instance);
            }
        }
    }
}