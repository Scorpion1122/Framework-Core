using System;
using UnityEngine;

namespace TKO.Core.Services
{
	public interface IServiceLocator
	{
	    T GetInstance<T>(bool allowNullService = true) where T : class;
	    T GetInstance<T>(object id, bool allowNullService = true) where T : class;
	    object GetInstance(Type type, object id, bool allowNullService = true);

	    Registration RegisterNullService<T, C>() where C : T;

	    Registration Register<T, C>() where C : T;
	    Registration Register<T>(IServiceFactory factory);

	    Registration Register<T, C>(object id) where C : T;
	    Registration Register<T>(object id, IServiceFactory factory);

	    Registration RegisterPrefab<T>(GameObject prefab) where T : Component;
	    Registration RegisterPrefab<T>(GameObject prefab, object id) where T : Component;

	    void RegisterInstance<T>(T instance);
	    void RegisterInstance<T>(T instance, object id);
	
	    void Remove<T>();
	    void Remove<T>(object id);
        void RemoveAll();
	}
}
