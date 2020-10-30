using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TKO.Core.Services
{
    public class PrefabFactory<C> : BaseServiceFactory where C : Component
    {
        private GameObject prefab;

        public PrefabFactory(GameObject prefab)
            : base(typeof(C))
        {
            this.prefab = prefab;
        }

        protected override object CreateInstanceObject()
        {
            GameObject prefabInstance = Object.Instantiate(prefab);
            return prefabInstance.AddComponent(instanceType);
        }

        public override void DisposeOfInstance(object instance)
        {
            Object objInstance = (Object) instance;
            if (Application.isPlaying)
                Object.Destroy(objInstance);
            else
                Object.DestroyImmediate(objInstance);
        }
    }
}
