using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimatorExpansion.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(AnimationEventReceiver))]
    internal class AnimationEventReceiverDrawer : Editor
    {
        private static Dictionary<Type, string> _attributeNameCacher = new Dictionary<Type, string>();

        private readonly Type _SEARCH_ATTRIBUTE_TYPE = typeof(AnimationEventAttribute);
        
        private const BindingFlags _BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public VisualElement visualElement;
        


        public void Awake()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                return;
            }
            
            AnimationEventReceiver receiver = selectedObject.GetComponent<AnimationEventReceiver>();

            foreach (var childComponent in selectedObject.GetComponentsInChildren<Component>())
            {
                foreach (var method in childComponent.GetType().GetMethods(_BINDING_FLAGS))
                {
                    if (method.GetCustomAttribute(_SEARCH_ATTRIBUTE_TYPE) is AnimationEventAttribute attribute)
                    {
                        int eventHash = Utility.StringToHash(attribute.eventName);
                        
                        receiver.eventReceiveInfoList.TryAdd(eventHash, new EventReceiveInfo()
                        {
                            eventName = attribute.eventName,
                            eventHash = eventHash,
                            parameterType = attribute.parameterType,
                        });
                    }
                }
            }

            foreach (var value in receiver.eventReceiveInfoList)
            {
                Debug.Log(value.Key + " " + value.Value.eventName);
            }
        }


        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            return root;
        }
    }
}