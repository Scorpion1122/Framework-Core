using System;
using System.Collections.Generic;
using UnityEngine;

namespace TKO.Core.Injection
{
    public static class Inject
    {
        private static readonly IInjectFactory[] injectFactories = new IInjectFactory[]
        {
            new ServiceInjectFactory(),
            new ComponentInjectFactory(),
            new ChildComponentInjectFactory(), 
            new ParentComponentFactory(),
        };
        
        private static readonly Dictionary<Type, bool> hasInjectablesCache = new Dictionary<Type, bool>();
        
        public static void Into(object target)
        {
            Type type = target.GetType();
            if (!HasInjectables(type))
                return;

            for (int i = 0; i < injectFactories.Length; i++)
            {
                IInjectFactory factory = injectFactories[i];
                if (!Application.isPlaying && !factory.AtEditTime)
                    continue;
                injectFactories[i].InjectInto(type, target);
            }
        }

        private static bool HasInjectables(Type type)
        {
            if (!hasInjectablesCache.ContainsKey(type))
            {
                for (int i = 0; i < injectFactories.Length; i++)
                {
                    if (injectFactories[i].HasRequiredAttributes(type))
                    {
                        hasInjectablesCache[type] = true;
                        break;
                    }

                    if (i == injectFactories.Length - 1)
                        hasInjectablesCache[type] = false;
                }
            }
            return hasInjectablesCache[type];
        }
    }
}