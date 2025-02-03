using System;
using System.Collections.Generic;
using System.Reflection;
using AnimatorExtension.Parameters;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimatorExtension
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationEventController : MonoBehaviour
    {
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
            ReflectionUtility.FindAttributeAction<AnimationEventAttribute>(this, (attribute, method, mono) =>
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



        private Delegate CreateDelegate(AnimationEventAttribute attribute, MethodInfo method, MonoBehaviour mono)
        {
            return attribute.eventParameter switch
            {
                EAnimationEventParameter.Void => method.CreateDelegate(ReflectionUtility.VoidType, mono),

                EAnimationEventParameter.Int => method.CreateDelegate(ReflectionUtility.IntType, mono),

                EAnimationEventParameter.Float => method.CreateDelegate(ReflectionUtility.FloatType, mono),

                EAnimationEventParameter.Bool => method.CreateDelegate(ReflectionUtility.BoolType, mono),

                EAnimationEventParameter.Tag => method.CreateDelegate(ReflectionUtility.StringType, mono),

                EAnimationEventParameter.String => method.CreateDelegate(ReflectionUtility.StringType, mono),

                EAnimationEventParameter.Color => method.CreateDelegate(ReflectionUtility.ColorType, mono),

                EAnimationEventParameter.LayerMask => method.CreateDelegate(ReflectionUtility.LayerMaskType, mono),

                EAnimationEventParameter.Vector2 => method.CreateDelegate(ReflectionUtility.Vector2Type, mono),

                EAnimationEventParameter.Vector3 => method.CreateDelegate(ReflectionUtility.Vector3Type, mono),

                EAnimationEventParameter.Quaternion => method.CreateDelegate(ReflectionUtility.QuaternionType, mono),

                EAnimationEventParameter.GameObject => method.CreateDelegate(ReflectionUtility.GameObjectType, mono),

                EAnimationEventParameter.Object => method.CreateDelegate(ReflectionUtility.ObjectType, mono),

                EAnimationEventParameter.AnimationCurve => method.CreateDelegate(ReflectionUtility.AnimationCurveType, mono),

                EAnimationEventParameter.Customization => method.CreateDelegate(ReflectionUtility.CustomizationType, mono),

                EAnimationEventParameter.AnimatorInfo => method.CreateDelegate(ReflectionUtility.AnimationInfoType, mono),
            };
        }
    }
}