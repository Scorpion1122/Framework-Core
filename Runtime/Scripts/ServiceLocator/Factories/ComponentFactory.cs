using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TKO.Core.Services
{
    public class ComponentFactory : BaseServiceFactory
    {
        public ComponentFactory(Type componentType) : base (componentType) { }

        protected override object CreateInstanceObject()
        {
            GameObject instanceObject = new GameObject(instanceType.Name + " (Instance)");
            return instanceObject.AddComponent(instanceType);
        }

        public override void DisposeOfInstance(object instance)
        {
            Component compInstance = (Component) instance;
            if (Application.isPlaying)
                Object.Destroy(compInstance);
            else
                Object.DestroyImmediate(compInstance);
        }
    }
}
