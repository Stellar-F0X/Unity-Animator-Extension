using System;
using System.Collections.Generic;
using System.Reflection;
using AnimatorExtension.Parameters;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AnimatorExtension
{
    public class ReflectionUtility
    {
        private const BindingFlags _EVENT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private readonly static Type _SearchAttributeType = typeof(AnimationEventAttribute);


        [DidReloadScripts]
        private static void OnReloadScripts()
        {
            
        }


        public static void SetEventsForContainer(AnimationEventReceiver receiver, EventInfoContainer container)
        {
            var components = receiver.GetComponentsInChildren<MonoBehaviour>(true);
            
            container.Clear();
            
            container.AddInfo("None", EAnimationEventParameter.Void, null);
            
            if (components is null)
            {
                return;
            }

            foreach (var component in components)
            {
                Type type = component.GetType();

                foreach (var method in type.GetMethods(_EVENT_BINDING_FLAGS))
                {
                    if (method.GetCustomAttribute(_SearchAttributeType) is AnimationEventAttribute attribute)
                    {
                        container.AddInfo(attribute.eventName, attribute.eventParameter, attribute.customParameterType);
                    }
                }
            }
            
            container.Build();
        }
    }
}