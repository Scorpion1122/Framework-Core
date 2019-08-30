using System;
using System.Collections.Generic;
using UnityEngine;
using ServiceGroup = System.Collections.Generic.Dictionary<object, Thijs.Core.Services.Registration>;
using ServicePair = System.Collections.Generic.KeyValuePair<object, Thijs.Core.Services.Registration>;

namespace Thijs.Core.Services
{
    public class ServiceLocator : IServiceLocator
    {
        private static ServiceLocator instance;
        public static ServiceLocator Instance
        {
            get
            {
                if (instance == null)
                    instance = new ServiceLocator();
                return instance;
            }
        }

        public const string NULL_SERVICE_KEY = "NULL";
        public const string DEFAULT_SERVICE_KEY = "DEFAULT";

        private static readonly Type COMPONENT_TYPE = typeof(Component);

        public delegate void OnInstanceStateDelegate(Type serviceType,
            object serviceInstance);

        private Dictionary<Type, ServiceGroup> register;

        public ServiceLocator()
        {
            register = new Dictionary<Type, ServiceGroup>();
        }

        #region Get
        /// <summary>
        /// Gets the desired default service (if no service found and allowNullService is true,
        /// return null service if one was registered)
        /// </summary>
        /// <typeparam name="T">The base type of the desired service (usually an interface)</typeparam>
        /// <param name="allowNullService">
        /// if the service is not found a null/mockup service might be returned (default is true)
        /// </param>
        /// <returns>Service of type T</returns>
        public T GetInstance<T>(bool allowNullService = true) where T : class
        {
            return GetInstance<T>(DEFAULT_SERVICE_KEY, allowNullService);
        }

        /// <summary>
        /// Gets the desired service based on ID
        /// </summary>
        /// <typeparam name="T">he base type of the desired service (usually an interface)</typeparam>
        /// <param name="id">The id that was used to register the service</param>
        /// <param name="allowNullService">
        /// if the service is not found a null/mockup service might be returned (default is true)
        /// </param>
        /// <returns>Service of type T</returns>
        public T GetInstance<T>(object id, bool allowNullService = true) where T : class
        {
            return GetInstance(typeof(T), id, allowNullService) as T;
        }

        /// <summary>
        /// Gets the desired service based on ID
        /// </summary>
        /// <param name="id">The id that was used to register the service</param>
        /// <param name="allowNullService">
        /// if the service is not found a null/mockup service might be returned (default is true)
        /// </param>
        /// <returns>Service</returns>
        public object GetInstance(Type type, object id, bool allowNullService = true)
        {
            object result = null;
            ServiceGroup serviceGroup;
            if (register.TryGetValue(type, out serviceGroup))
            {
                if (serviceGroup.ContainsKey(id))
                    result = serviceGroup[id].GetInstance(this);
                if (allowNullService && result == null && serviceGroup.ContainsKey(NULL_SERVICE_KEY))
                    result = serviceGroup[NULL_SERVICE_KEY].GetInstance(this);
            }

            if (result == null)
                throw new Exception(string.Format("Service of type {0}, id {1} was not found!", type, id));
            return result;
        }
        #endregion Get

        #region Register
        /// <summary>
        /// Registers a service as a NULL Service, this service will be used if a concrete
        /// implementation was not registerd
        /// </summary>
        /// <typeparam name="T">the template type (used to retrieve the service)</typeparam>
        /// <typeparam name="C">the concrete implementation of T</typeparam>
        public Registration RegisterNullService<T, C>() where C : T
        {
            return Register<T, C>(NULL_SERVICE_KEY);
        }

        /// <summary>
        /// Register a service based on tempate T and concrete C based on an id
        /// - C must implement T
        /// - No other service must be registered with the same T/id C will only be instatiated when
        /// the service is requested (Lazy Initialization)
        /// </summary>
        /// <typeparam name="T">the template type (used to retrieve the service)</typeparam>
        /// <typeparam name="C">the concrete implementation of T</typeparam>
        public Registration Register<T, C>() where C : T
        {
            return Register<T, C>(DEFAULT_SERVICE_KEY);
        }

        public Registration Register<T>(IServiceFactory factory)
        {
            return Register<T>(DEFAULT_SERVICE_KEY, factory);
        }

        /// <summary>
        /// Register a service based on tempate T and concrete C based on an id
        /// - C must implement T
        /// - No other service must be registered with the same T/id C will only be instatiated when
        /// the service is requested (Lazy Initialization)
        /// </summary>
        /// <typeparam name="T">the template type (used to retrieve the service)</typeparam>
        /// <typeparam name="C">the concrete implementation of T</typeparam>
        /// <param name="id">registartion id (used to retrieve a specific service)</param>
        public Registration Register<T, C>(object id)  where C : T
        {
            Type concreteType = typeof(C);
            if (IsComponent(concreteType))
                return Register<T>(id, new ComponentFactory(concreteType));
            return Register<T>(id, new ClassFactory(concreteType));
        }

        public Registration Register<T>(object id, IServiceFactory factory)
        {
            Type type = typeof(T);
            if (CanAddService(type, id))
            {
                Registration registration = new Registration(factory);
                register[type].Add(id, registration);
                return registration;
            }
            throw new Exception(string.Format("Service {0} could not be added!", type.Name));
        }

        /// <summary>
        /// Register a prefab as a service
        /// - prefab must implement type T
        /// - No other service must be registered with the same T/id The prefab will only be
        /// instantiated when the service is requested
        /// </summary>
        /// <typeparam name="T">the template type (used to retrieve the service)</typeparam>
        /// <param name="prefab">the prefab that will supply the service instance</param>
        public Registration RegisterPrefab<T>(GameObject prefab) where T : Component
        {
            return RegisterPrefab<T>(prefab, DEFAULT_SERVICE_KEY);
        }

        /// <summary>
        /// Register a prefab as a service
        /// - prefab must implement type T
        /// - No other service must be registered with the same T/id The prefab will only be
        /// instantiated when the service is requested
        /// </summary>
        /// <typeparam name="T">the template type (used to retrieve the service)</typeparam>
        /// <param name="prefab">the prefab that will supply the service instance</param>
        /// <param name="id">registartion id (used to retrieve a specific service)</param>
        public Registration RegisterPrefab<T>(GameObject prefab, object id) where T : Component
        {
            Type type = typeof(T);
            if (CanAddService(type, id))
            {
                Registration registration = new Registration(new PrefabFactory<T>(prefab));
                register[type].Add(id, registration);
                return registration;
            }
            throw new Exception(string.Format("Service <{0},{1}> could not be registered", type, prefab));
        }

        /// <summary>
        /// Registers an instance of a service
        /// - No other service must be registered with the same T/id
        /// </summary>
        /// <typeparam name="T">the template type (used to retrieve the service)</typeparam>
        /// <param name="instance">the instance of a service that implements T</param>
        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance<T>(instance, DEFAULT_SERVICE_KEY);
        }

        public void RegisterInstance(Type type, object instance)
        {
            RegisterInstance(type, instance, DEFAULT_SERVICE_KEY);
        }

        /// <summary>
        /// Registers an instance of a service
        /// - No other service must be registered with the same T/id
        /// </summary>
        /// <typeparam name="T">the template type (used to retrieve the service)</typeparam>
        /// <param name="instance">the instance of a service that implements T</param>
        /// <param name="id">registartion id (used to retrieve a specific service)</param>
        public void RegisterInstance<T>(T instance, object id)
        {
            Type type = typeof(T);
            RegisterInstance(type, instance, id);
        }

        public void RegisterInstance(Type type, object instance, object id)
        {
            if (CanAddService(type, id))
            {
                if (IsComponent(instance.GetType()))
                    register[type].Add(id, new Registration(instance, new ComponentFactory(type)));
                else
                    register[type].Add(id, new Registration(instance, new ClassFactory(type)));
            }
        }
        #endregion Register

        #region Remove
        public void Remove<T>()
        {
            Remove<T>(DEFAULT_SERVICE_KEY);
        }

        public void Remove(Type type)
        {
            Remove(type, DEFAULT_SERVICE_KEY);
        }

        public void Remove<T>(object id)
        {
            Remove(typeof(T), id);
        }

        public void Remove(Type type, object id)
        {
            if (HasService(type, id))
            {
                register[type][id].Dispose();
                register[type].Remove(id);
            }
        }

        /// <summary>
        /// Removes and Disposes all the registered service (including null services)
        /// </summary>
        public void RemoveAll()
        {
            foreach (KeyValuePair<Type, ServiceGroup> serviceGroupPair in register)
                foreach (ServicePair servicePair in serviceGroupPair.Value)
                    servicePair.Value.Dispose();
            register.Clear();
        }
        #endregion Remove

        #region Helpers
        private bool CanAddService(Type templateType, object id)
        {
            if (HasService(templateType, id))
            {
                Debug.LogError(string.Format("Service of type: {0} - id: {1}, is already registered!", templateType, id));
                return false;
            }
            if (!register.ContainsKey(templateType))
                register.Add(templateType, new ServiceGroup());
            return true;
        }

        public bool HasService<T>()
        {
            return HasService(typeof(T), DEFAULT_SERVICE_KEY);
        }

        private bool HasService(Type type, object id)
        {
            return (register.ContainsKey(type) && register[type].ContainsKey(id));
        }

        private bool IsComponent(Type type)
        {
            return COMPONENT_TYPE.IsAssignableFrom(type);
        }
        #endregion
    }
}
