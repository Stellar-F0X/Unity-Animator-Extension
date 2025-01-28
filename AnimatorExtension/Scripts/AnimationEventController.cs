using System;
using System.Collections.Generic;
using System.Reflection;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationEventController : MonoBehaviour
    {
        private readonly Dictionary<int, AnimationEventCallback> _eventList = new Dictionary<int, AnimationEventCallback>();


        public bool debug = false;

        private Animator _animator;


        public Animator animator
        {
            get { return _animator == null ? _animator = GetComponent<Animator>() : _animator; }
        }


        private void Awake()
        {
            ReflectionUtility.FindAttributeAction<AnimationEventAttribute>(this, (attribute, method, mono) =>
            {
                int eventHash = Extension.StringToHash(attribute.eventName);

                Delegate createdCallback = this.CreateDelegate(attribute, method, mono);

                bool succeed = _eventList.TryAdd(eventHash, new AnimationEventCallback(createdCallback));

                if (debug)
                {
                    if (succeed)
                    {
                        Debug.Log($"<color=green>[Animation Event Receiver]</color> Register {attribute.eventName} animation event");
                    }
                    else
                    {
                        Debug.LogError($"<color=green>[Animation Event Receiver]</color> Failed registry {attribute.eventName} animation event");
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
                    Debug.Log($"<color=green>[Animation Event Receiver]</color> Received {eventName} event");
                }
                else
                {
                    Debug.LogError($"color=green>[Animation Event Receiver]</color> {eventName} event does not exist.");
                }
            }

            if (succeed)
            {
                eventCallback.Invoke(animationEventParameter);
            }
        }



        public void AddRuntimeEvent<T>(string eventName, int targetLayer, AnimatorStateInfo stateInfo, AnimationEventParameter param, Action<T> action)
        {
            int eventHash = Extension.StringToHash(eventName);

            bool succeed = _eventList.TryAdd(eventHash, new AnimationEventCallback(action, true));
        }



        public void AddRuntimeEvent(string eventName, int targetLayer, AnimatorStateInfo stateInfo, Action action)
        {
            int eventHash = Extension.StringToHash(eventName);

            bool succeed = _eventList.TryAdd(eventHash, new AnimationEventCallback(action, true));
        }



        public void RemoveRuntimeEvent(string eventName, int targetLayer)
        {

        }



        public void DisableEvent(string eventName, int targetLayer, AnimatorStateInfo stateInfo)
        {

        }



        public void EnableEvent(string eventName, int targetLayer, AnimatorStateInfo stateInfo)
        {

        }



        public void RegisterSequence()
        {
            
        }



        public void RemoveSequences()
        {
            
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

                EAnimationEventParameter.ScriptableObject => method.CreateDelegate(ReflectionUtility.ScriptableObjectType, mono),

                EAnimationEventParameter.AnimationCurve => method.CreateDelegate(ReflectionUtility.AnimationCurveType, mono),

                EAnimationEventParameter.Customization => method.CreateDelegate(ReflectionUtility.CustomizationType, mono),
            };
        }
    }
}