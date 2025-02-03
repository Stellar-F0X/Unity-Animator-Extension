using System;
using System.Reflection;
using AnimatorExtension.Parameters;
using UnityEngine;
using AnimationInfo = AnimatorExtension.Parameters.AnimationInfo;
using Object = UnityEngine.Object;

namespace AnimatorExtension
{
    internal class ReflectionUtility
    {
        public static readonly Type VoidType = typeof(Action);
        
        public static readonly Type IntType = typeof(Action<int>);
        
        public static readonly Type FloatType = typeof(Action<float>);
        
        public static readonly Type BoolType = typeof(Action<bool>);
        
        public static readonly Type StringType = typeof(Action<string>);
        
        public static readonly Type ColorType = typeof(Action<Color>);
        
        public static readonly Type LayerMaskType = typeof(Action<LayerMask>);
        
        public static readonly Type Vector2Type = typeof(Action<Vector2>);
        
        public static readonly Type Vector3Type = typeof(Action<Vector3>);
        
        public static readonly Type QuaternionType = typeof(Action<Quaternion>);
        
        public static readonly Type GameObjectType = typeof(Action<GameObject>);
        
        public static readonly Type ObjectType = typeof(Action<Object>);
        
        public static readonly Type AnimationCurveType = typeof(Action<AnimationCurve>);
        
        public static readonly Type AnimationInfoType = typeof(Action<AnimationInfo>);
        
        public static readonly Type CustomizationType = typeof(Action<CustomAnimationEventParameter>);
        
        
        
        private const BindingFlags _EVENT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly static Type _SEARCH_ATTRIBUTE_TYPE = typeof(AnimationEventAttribute);

        


        public static void SetEventsForContainer(AnimationEventController controller, EventInfoContainer container)
        {
            container.Clear();

            if (controller.GetComponentsInChildren<MonoBehaviour>(true) is MonoBehaviour[] components)
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


        public static void FindAttributeAction<T>(AnimationEventController controller, Action<T, MethodInfo, MonoBehaviour> action) where T : Attribute
        {
            MonoBehaviour[] components = controller.GetComponentsInChildren<MonoBehaviour>(true);
            
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