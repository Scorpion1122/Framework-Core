using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TKO.Core.Services
{
    public static class StaticServiceReporter
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitializeOnEditorLoad()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                RegisterEditTimeServices();
        }

        private static void OnPlayModeChanged(PlayModeStateChange playMode)
        {
            if (playMode == PlayModeStateChange.ExitingEditMode)
                ReleaseEditTimeServices();
            if (playMode == PlayModeStateChange.ExitingPlayMode)
            {
                ReleasePlayTimeServices();
                RegisterEditTimeServices();
            }
        }

        private static void RegisterEditTimeServices()
        {
            Register(GetServiceReporters<IEditorServiceReporter>());
        }

        private static void ReleaseEditTimeServices()
        {
            Release(GetServiceReporters<IEditorServiceReporter>());
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterPlayTimeServices()
        {
            Register(GetServiceReporters<IRuntimeServiceReporter>());
        }

        private static void ReleasePlayTimeServices()
        {
            Release(GetServiceReporters<IRuntimeServiceReporter>());
        }

        private static void Register(List<IServiceReporter> reporters)
        {
            for (int i = 0; i < reporters.Count; i++)
                reporters[i].RegisterServices(ServiceLocator.Instance);
        }

        private static void Release(List<IServiceReporter> reporters)
        {
            for (int i = 0; i < reporters.Count; i++)
                reporters[i].ReleaseServices(ServiceLocator.Instance);
        }

        private static List<IServiceReporter> GetServiceReporters<T>()
            where T : IServiceReporter
        {
            List<IServiceReporter> result = new List<IServiceReporter>();
            Type iType = typeof(T);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int t = 0; t < types.Length; t++)
                {
                    Type type = types[t];
                    if (type.IsAbstract)
                        continue;
                    if (iType.IsAssignableFrom(type))
                        result.Add((IServiceReporter) Activator.CreateInstance(type));
                }
            }

            return result;
        }
    }
}