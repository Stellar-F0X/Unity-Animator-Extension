using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    public static class ReflectionUtility
    {
        private const BindingFlags _BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private readonly static Type _SEARCH_ATTRIBUTE_TYPE = typeof(AnimationEventAttribute);

        private readonly static Dictionary<Type, Func<MonoBehaviour, IEnumerable<string>>> _methodCache = new Dictionary<Type, Func<MonoBehaviour, IEnumerable<string>>>();

        private readonly static Lazy<List<string>> _cachedEventName = new Lazy<List<string>>(() => new List<string>());
        
        private readonly static Lazy<List<string>> _tempCachedEventNames = new Lazy<List<string>>(() => new List<string>());
        
        
        [DidReloadScripts]
        private static void OnReloadScripts()
        {
            _methodCache.Clear();
            _cachedEventName.Value.Clear();
            _tempCachedEventNames.Value.Clear();
        }
        

        public static void GetAnimationEventNames(AnimationEventReceiver receiver, out string[] nameList)
        {
            var components = receiver.GetComponentsInChildren<MonoBehaviour>(true);

            if (components == null || components.Length == 0)
            {
                nameList = null;
                return;
            }

            _cachedEventName.Value.Clear();

            foreach (var component in components)
            {
                var type = component.GetType();
                
                if (!_methodCache.TryGetValue(type, out var getMethodNames))
                {
                    getMethodNames = CreateMethodDelegate(type);
                    
                    _methodCache[type] = getMethodNames;
                }

                _cachedEventName.Value.AddRange(getMethodNames(component));
            }

            nameList = _cachedEventName.Value.ToArray();
        }


        private static Func<MonoBehaviour, IEnumerable<string>> CreateMethodDelegate(Type type)
        {
            _tempCachedEventNames.Value.Clear();

            foreach (var method in type.GetMethods(_BINDING_FLAGS))
            {
                if (method.GetCustomAttribute(_SEARCH_ATTRIBUTE_TYPE) is AnimationEventAttribute attribute)
                {
                    _tempCachedEventNames.Value.Add(attribute.eventName);
                }
            }

            return _ => _tempCachedEventNames.Value;
        }
    }
}