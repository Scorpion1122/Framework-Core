using System;
using System.Collections.Generic;
using System.Reflection;

namespace Thijs.Core.Services
{
    /// <summary>
    /// Registers services that are declared in a subclass of this class and are marked with the
    /// [ImplementsService]-attribute. (See also, ImplementsServiceAttribute)
    /// </summary>
    public abstract class ReflectiveMonoServiceReporter : MonoServiceReporter
    {

        private Dictionary<Type, object> serviceToImplementation;

        /// <summary>
        /// Find all fields in this class that have one or more ImplementsServiceAttribute's
        /// </summary>
        private void FindServicesInFields()
        {
            serviceToImplementation = new Dictionary<Type, object>();
            FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object[] attributes = field.GetCustomAttributes(typeof(ImplementsMonoServiceAttribute), false);

                foreach (object attribute in attributes)
                {
                    ImplementsMonoServiceAttribute implementsMonoServiceAttribute
                        = (ImplementsMonoServiceAttribute)attribute;

                    if (!implementsMonoServiceAttribute.InterfaceType.IsAssignableFrom(field.FieldType))
                    {
                        throw new Exception("Service field " + field.Name + " does not implement "
                            + "service " + implementsMonoServiceAttribute.InterfaceType.Name);
                    }

                    serviceToImplementation[implementsMonoServiceAttribute.InterfaceType] = field.GetValue(this);
                }
            }
        }

        protected override void Awake()
        {
            // First find the services in this class, then call Awake, because we need to know which
            // services to register in order to be able to register them
            FindServicesInFields();
            base.Awake();
        }

        protected override void RegisterServices(ServiceLocator locator)
        {
            foreach (Type serviceType in serviceToImplementation.Keys)
            {
                locator.RegisterInstance(serviceType, serviceToImplementation[serviceType]);
            }
        }

        protected override void RemoveServices(ServiceLocator locator)
        {
            if (serviceToImplementation == null)
            {
                return;
            }
            foreach (Type serviceType in serviceToImplementation.Keys)
            {
                locator.Remove(serviceType);
            }
        }
    }
}
