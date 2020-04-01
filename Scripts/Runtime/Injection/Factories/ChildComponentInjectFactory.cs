using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TKO.Core.Injection
{
    public class ChildComponentInjectFactory : InjectFactory<ChildAttribute>
    {
        public override bool AtEditTime => true;
        public override bool MustBeMonobehaviour { get { return true; } }
        
        protected override void InjectInto(Type type, InjectDefinition<ChildAttribute> definition, object target)
        {
            GameObject gameObject = ((MonoBehaviour)target).gameObject;
            foreach (KeyValuePair<FieldInfo, ChildAttribute> pair in definition.Fields)
            {
                Type injectType = pair.Value.InjectType ?? pair.Key.FieldType;
                Component instance = gameObject.GetComponentInChildren(injectType);
                pair.Key.SetValue(target, instance);
            }
        }
    }
}