using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TKO.Core.Injection
{
    public class InjectDefinition<T> where T : Attribute
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags BINDING_FLAGS_PRIVATE = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly Type MONO_TYPE = typeof(MonoBehaviour);
        
        private Type attributeType;
        private readonly Dictionary<FieldInfo, T> fields;

        public Dictionary<FieldInfo, T> Fields { get { return fields; } }

        public InjectDefinition()
        {
            attributeType = typeof(T);
            fields = new Dictionary<FieldInfo, T>();
        }
        
        public void AddTypeInfo(Type type)
        {
            AddInjectableMembers(type, BINDING_FLAGS);
            AddInjectableInheritedMembers(type.BaseType);
        }
        
        private void AddInjectableInheritedMembers(Type currentType)
        {
            if (currentType == null || currentType == MONO_TYPE)
                return;

            AddInjectableMembers(currentType, BINDING_FLAGS_PRIVATE);
            AddInjectableInheritedMembers(currentType.BaseType);
        }

        private void AddInjectableMembers(Type type, BindingFlags flags)
        {
            MemberInfo[] infos = type.GetMembers(flags);
            for (int i = 0; i < infos.Length; i++)
            {
                object[] attributes = infos[i].GetCustomAttributes(attributeType, true);
                if (attributes.Length != 0)
                    AddMemberInfo(infos[i], (T)attributes[0]);
            }
        }

        private void AddMemberInfo(MemberInfo info, T attribute)
        {
            FieldInfo field = info as FieldInfo;
            if (field != null)
            {
                AddField(field, attribute);
            }
        }

        private void AddField(FieldInfo info, T attribute)
        {
            fields[info] = attribute;
        }

    }
}