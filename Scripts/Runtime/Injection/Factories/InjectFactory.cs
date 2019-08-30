using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thijs.Core.Injection
{
    public abstract class InjectFactory<T> : IInjectFactory where T : Attribute
    {
        protected static readonly Type MONO_TYPE = typeof(MonoBehaviour);
        
        public abstract bool AtEditTime { get; }
        
        public Type RequiredAttribute { get { return typeof(T); } }
        public abstract bool MustBeMonobehaviour { get; }
        
        private Dictionary<Type, InjectDefinition<T>> definitions 
            = new Dictionary<Type, InjectDefinition<T>>();
        
        public bool HasRequiredAttributes(Type type)
        {
            if (MustBeMonobehaviour && !MONO_TYPE.IsAssignableFrom(type))
                return false;
            return true;
        }

        public void InjectInto(Type type, object target)
        {
            InjectInto(type, GetInjectDefinition(type), target);
        }

        protected abstract void InjectInto(Type type, InjectDefinition<T> definition, object target);
        
        private InjectDefinition<T> GetInjectDefinition(Type type)
        {
            if (!definitions.ContainsKey(type))
                definitions[type] = CreateInjectDefinition(type);
            return definitions[type];
        }

        private InjectDefinition<T> CreateInjectDefinition(Type type)
        {
            InjectDefinition<T> definition = new InjectDefinition<T>();
            definition.AddTypeInfo(type);
            return definition;
        }
    }
}