using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Thijs.Core.Injection
{
    public class ParentComponentFactory : InjectFactory<ParentAttribute>
    {
        public override bool AtEditTime => true;
        public override bool MustBeMonobehaviour { get { return true; } }
        
        protected override void InjectInto(Type type, InjectDefinition<ParentAttribute> definition, object target)
        {
            GameObject gameObject = ((MonoBehaviour)target).gameObject;
            foreach (KeyValuePair<FieldInfo, ParentAttribute> pair in definition.Fields)
            {
                Type injectType = pair.Value.InjectType ?? pair.Key.FieldType;
                Component instance = gameObject.GetComponentInParent(injectType);
                pair.Key.SetValue(target, instance);
            }
        }
    }
}