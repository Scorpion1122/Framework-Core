using UnityEngine;

namespace Thijs.Core.Services
{
    /// <summary>
    /// Monobehaviour that is used to register services into the service locator. 
    /// This baseclass can be used to setup some default services (null services)
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public abstract class MonoServiceReporter : MonoBehaviour
    {
        protected virtual void Awake()
        {
            RegisterNullServices(ServiceLocator.Instance);
            RegisterServices(ServiceLocator.Instance);
        }

        protected virtual void RegisterNullServices(ServiceLocator locator)
        {
        }

        protected abstract void RegisterServices(ServiceLocator locator);

        protected void RegisterChildService<T>(ServiceLocator locator)
            where T : Component
        {
            T component = GetComponentInChildren<T>();
            if (component != null)
                locator.RegisterInstance<T>(component);
            else
                Debug.LogErrorFormat("Component service of type {0} not found!", typeof(T));
        }

        protected virtual void OnDestroy()
        {
            RemoveNullServices(ServiceLocator.Instance);
            RemoveServices(ServiceLocator.Instance);
        }

        protected virtual void RemoveNullServices(ServiceLocator locator)
        {
        }

        protected abstract void RemoveServices(ServiceLocator locator);

        protected void RegisterChildInstance<T>(ServiceLocator locator)
        {
            T instance = GetComponentInChildren<T>();
            if (instance != null)
                locator.RegisterInstance(instance);
        }
    }

}
