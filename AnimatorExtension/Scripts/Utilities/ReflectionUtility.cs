using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AnimatorExtension.Parameters;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AnimatorExtension
{
    internal class ReflectionUtility
    {
        private const BindingFlags _EVENT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly static Type _SEARCH_ATTRIBUTE_TYPE = typeof(AnimationEventAttribute);



        [DidReloadScripts]
        private static void OnReloadScripts()
        {

        }


        public static void SetEventsForContainer(AnimationEventReceiver receiver, EventInfoContainer container)
        {
            container.Clear();

            if (receiver.GetComponentsInChildren<MonoBehaviour>(true) is MonoBehaviour[] components)
            {
                foreach (var mono in components)
                {
                    Type currentMonoType = mono.GetType();

                    foreach (var method in currentMonoType.GetMethods(_EVENT_BINDING_FLAGS))
                    {
                        if (method.GetCustomAttribute(_SEARCH_ATTRIBUTE_TYPE) is AnimationEventAttribute attribute)
                        {
                            container.AddInfo(attribute);
                        }
                    }
                }

                container.Build();
            }
        }


        public static void FindAttributeAction<T>(AnimationEventReceiver receiver, Action<T, MethodInfo, MonoBehaviour> action) where T : Attribute
        {
            MonoBehaviour[] components = receiver.GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var mono in components)
            {
                Type currentMonoType = mono.GetType();

                foreach (var method in currentMonoType.GetMethods(_EVENT_BINDING_FLAGS))
                {
                    if (method.GetCustomAttribute(_SEARCH_ATTRIBUTE_TYPE) is T attribute)
                    {
                        action.Invoke(attribute, method, mono);
                    }
                }
            }
        }
    }
}