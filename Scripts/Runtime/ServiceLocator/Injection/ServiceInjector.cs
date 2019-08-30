using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Thijs.Core.Services
{
    public class ServiceInjector
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags BINDING_FLAGS_PRIVATE = BindingFlags.Instance | BindingFlags.NonPublic;

        private static readonly Type INJECT_ATTRIBUTE_TYPE = typeof(InjectAttribute);
        private static readonly Type INJECTABLE_ATTRIBUTE_TYPE = typeof(InjectableAttribute);
        private static readonly Type MONO_TYPE = typeof(MonoBehaviour);

        private static Dictionary<Type, InjectableDefinition> definitions
            = new Dictionary<Type, InjectableDefinition>();

        private static Dictionary<Type, bool> injectables
            = new Dictionary<Type, bool>();

        public static bool HasInjectableAttribute(Type type)
        {
            if (!injectables.ContainsKey(type))
            {
                object[] attributes = type.GetCustomAttributes(INJECTABLE_ATTRIBUTE_TYPE, true);
                injectables[type] = (attributes.Length != 0);
            }
            return injectables[type];
        }

        public static void InjectInto(object target)
        {
            InjectInto(ServiceLocator.Instance, target);
        }

        public static void InjectInto(IServiceLocator locator, object target)
        {
            InjectableDefinition definition = GetInjectDefinition(target);
            foreach (KeyValuePair<FieldInfo, InjectAttribute> pair in definition.Fields)
            {
                Type type = pair.Value.InjectType ?? pair.Key.FieldType;
                object id = pair.Value.Id ?? ServiceLocator.DEFAULT_SERVICE_KEY;
                object instance = locator.GetInstance(type, id);
                pair.Key.SetValue(target, instance);
            }

            foreach (KeyValuePair<PropertyInfo, InjectAttribute> pair in definition.Properties)
            {
                Type type = pair.Value.InjectType ?? pair.Key.PropertyType;
                object id = pair.Value.Id ?? ServiceLocator.DEFAULT_SERVICE_KEY;
                object instance = locator.GetInstance(type, id);
                pair.Key.SetValue(target, instance, null);
            }
        }

        private static InjectableDefinition GetInjectDefinition(object target)
        {
            Type type = target.GetType();
            if (!definitions.ContainsKey(type))
                definitions[type] = CreateInjectDefinition(type);
            return definitions[type];
        }

        private static InjectableDefinition CreateInjectDefinition(Type type)
        {
            InjectableDefinition definition = new InjectableDefinition();
            AddInjectableMembers(definition, type, BINDING_FLAGS);
            AddInjectableInheritedMembers(definition, type.BaseType);
            return definition;
        }

        private static void AddInjectableInheritedMembers(InjectableDefinition definition, Type currentType)
        {
            if (currentType == null || currentType == MONO_TYPE)
                return;

            AddInjectableMembers(definition, currentType, BINDING_FLAGS_PRIVATE);
            AddInjectableInheritedMembers(definition, currentType.BaseType);
        }

        private static void AddInjectableMembers(InjectableDefinition definition, Type type, BindingFlags flags)
        {
            MemberInfo[] infos = type.GetMembers(flags);
            for (int i = 0; i < infos.Length; i++)
            {
                object[] attributes = infos[i].GetCustomAttributes(INJECT_ATTRIBUTE_TYPE, true);
                if (attributes.Length != 0)
                    definition.Add(infos[i], (InjectAttribute)attributes[0]);
            }
        }
    }
}
