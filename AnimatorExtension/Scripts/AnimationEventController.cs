using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using AnimatorExtension.Parameters;

namespace AnimatorExtension
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationEventController : MonoBehaviour
    {
        #region Parameter Types

        private static readonly Type _voidType = typeof(Action);
        
        private static readonly Type _intType = typeof(Action<int>);
        
        private static readonly Type _floatType = typeof(Action<float>);
        
        private static readonly Type _boolType = typeof(Action<bool>);
        
        private static readonly Type _stringType = typeof(Action<string>);
        
        private static readonly Type _colorType = typeof(Action<Color>);
        
        private static readonly Type _layerMaskType = typeof(Action<LayerMask>);
        
        private static readonly Type _vector2Type = typeof(Action<Vector2>);
        
        private static readonly Type _vector3Type = typeof(Action<Vector3>);
        
        private static readonly Type _quaternionType = typeof(Action<Quaternion>);
        
        private static readonly Type _gameObjectType = typeof(Action<GameObject>);
        
        private static readonly Type _objectType = typeof(Action<UnityEngine.Object>);
        
        private static readonly Type _animationCurveType = typeof(Action<AnimationCurve>);
        
        private static readonly Type _animationInfoType = typeof(Action<Parameters.AnimationInfo>);
        
        private static readonly Type _customizationType = typeof(Action<CustomAnimationEventParameter>);

        private static readonly Type _searchAttributeType = typeof(AnimationEventAttribute);
        
        #endregion
        
        
        private const BindingFlags _EVENT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private readonly Dictionary<int, AnimationEventCallback> _eventList = new Dictionary<int, AnimationEventCallback>();
        
        
        public bool debug = false;

        private Animator _animator;

        private const string _INTRO = "<color=green>[Animation Event Receiver]</color>";

        
        public Dictionary<int, AnimationEventCallback>.ValueCollection eventList
        {
            get { return _eventList.Values; }
        }

        public Animator animator
        {
            get { return _animator == null ? _animator = GetComponent<Animator>() : _animator; }
        }

        

        private void Awake()
        {
            this.FindAttributeAction<AnimationEventAttribute>(this, (attribute, method, mono) =>
            {
                int eventHash = attribute.eventName.StringToHash();

                Delegate createdCallback = this.CreateDelegate(attribute, method, mono);

                bool succeed = _eventList.TryAdd(eventHash, new AnimationEventCallback(attribute.eventName, createdCallback));

                if (debug)
                {
                    if (succeed)
                    {
                        Debug.Log($"{_INTRO} Register {attribute.eventName} animation event");
                    }
                    else
                    {
                        Debug.LogError($"{_INTRO} Failed registry {attribute.eventName} animation event");
                    }
                }
            });
        }



        public void ReceiveEvent(string eventName, int eventHash, AnimationEventParameter animationEventParameter)
        {
            bool succeed = _eventList.TryGetValue(eventHash, out AnimationEventCallback eventCallback);

            if (debug)
            {
                if (succeed)
                {
                    Debug.Log($"{_INTRO} Received {eventCallback.callbackName} event");
                }
                else
                {
                    Debug.LogError($"{_INTRO} {eventName} event does not exist.");
                }
            }

            if (succeed)
            {
                eventCallback.Invoke(animationEventParameter);
            }
        }



        public void SetActiveEvent(string eventName, bool active)
        {
            int eventHash = eventName.StringToHash();

            this.SetActiveEvent(eventHash, active);
        }
        
        
        
        public void SetActiveEvent(int eventHash, bool active)
        {
            bool hasEvent = _eventList.ContainsKey(eventHash);

            if (hasEvent)
            {
                _eventList[eventHash].enable = active;
            }

            if (debug)
            {
                if (hasEvent)
                {
                    string eventName = _eventList[eventHash].callbackName;
                    
                    Debug.Log($"{_INTRO} {(active ? "Enable" : "Disable")} {eventName} event");
                }
                else
                {
                    Debug.Log($"{_INTRO} Event does not exist.");
                }
            }
        }
        
        
        
        private void FindAttributeAction<T>(AnimationEventController controller, Action<T, MethodInfo, MonoBehaviour> action) where T : Attribute
        {
            MonoBehaviour[] components = controller.GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (var mono in components)
            {
                Type currentMonoType = mono.GetType();

                foreach (var method in currentMonoType.GetMethods(_EVENT_BINDING_FLAGS))
                {
                    if (method.GetCustomAttribute(_searchAttributeType) is T attribute)
                    {
                        action.Invoke(attribute, method, mono);
                    }
                }
            }
        }



        private Delegate CreateDelegate(AnimationEventAttribute attribute, MethodInfo method, MonoBehaviour mono)
        {
            return attribute.eventParameter switch
            {
                EAnimationEventParameter.Void => method.CreateDelegate(_voidType, mono),

                EAnimationEventParameter.Int => method.CreateDelegate(_intType, mono),

                EAnimationEventParameter.Float => method.CreateDelegate(_floatType, mono),

                EAnimationEventParameter.Bool => method.CreateDelegate(_boolType, mono),

                EAnimationEventParameter.Tag => method.CreateDelegate(_stringType, mono),

                EAnimationEventParameter.String => method.CreateDelegate(_stringType, mono),

                EAnimationEventParameter.Color => method.CreateDelegate(_colorType, mono),

                EAnimationEventParameter.LayerMask => method.CreateDelegate(_layerMaskType, mono),

                EAnimationEventParameter.Vector2 => method.CreateDelegate(_vector2Type, mono),

                EAnimationEventParameter.Vector3 => method.CreateDelegate(_vector3Type, mono),

                EAnimationEventParameter.Quaternion => method.CreateDelegate(_quaternionType, mono),

                EAnimationEventParameter.GameObject => method.CreateDelegate(_gameObjectType, mono),

                EAnimationEventParameter.Object => method.CreateDelegate(_objectType, mono),

                EAnimationEventParameter.AnimationCurve => method.CreateDelegate(_animationCurveType, mono),

                EAnimationEventParameter.Customization => method.CreateDelegate(_customizationType, mono),

                EAnimationEventParameter.AnimatorInfo => method.CreateDelegate(_animationInfoType, mono)
            };
        }
    }
}