using System;
using System.Reflection;
using AnimatorExtension.Parameters;
using AnimatorExtension.Sample;
using UnityEngine;

namespace AnimatorExtension
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationEventReceiver : MonoBehaviour
    {
        public bool debug = false;
        
        private readonly EventContainer _eventContainer = new EventContainer();

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

                EAnimationEventParameter paramType = attribute.eventParameter;

                switch (paramType)
                {
                    case EAnimationEventParameter.Void:
                    {
                        Action action = (Action)method.CreateDelegate(typeof(Action), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Int:
                    {
                        Action<int> action = (Action<int>)method.CreateDelegate(typeof(Action<int>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<int>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Float:
                    {
                        Action<float> action = (Action<float>)method.CreateDelegate(typeof(Action<float>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<float>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Bool:
                    {
                        Action<bool> action = (Action<bool>)method.CreateDelegate(typeof(Action<bool>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<bool>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Tag:
                    case EAnimationEventParameter.String:
                    {
                        Action<string> action = (Action<string>)method.CreateDelegate(typeof(Action<string>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<string>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Color:
                    {
                        Action<Color> action = (Action<Color>)method.CreateDelegate(typeof(Action<Color>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<Color>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.LayerMask:
                    {
                        Action<LayerMask> action = (Action<LayerMask>)method.CreateDelegate(typeof(Action<LayerMask>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<LayerMask>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Vector2:
                    {
                        Action<Vector2> action = (Action<Vector2>)method.CreateDelegate(typeof(Action<Vector2>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<Vector2>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Vector3:
                    {
                        Action<Vector3> action = (Action<Vector3>)method.CreateDelegate(typeof(Action<Vector3>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<Vector3>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Quaternion:
                    {
                        Action<Quaternion> action = (Action<Quaternion>)method.CreateDelegate(typeof(Action<Quaternion>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<Quaternion>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.GameObject:
                    {
                        Action<GameObject> action = (Action<GameObject>)method.CreateDelegate(typeof(Action<GameObject>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<GameObject>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.ScriptableObject:
                    {
                        Action<ScriptableObject> action = (Action<ScriptableObject>)method.CreateDelegate(typeof(Action<ScriptableObject>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<ScriptableObject>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.AnimationCurve:
                    {
                        Action<AnimationCurve> action = (Action<AnimationCurve>)method.CreateDelegate(typeof(Action<AnimationCurve>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<AnimationCurve>(action, paramType));
                        break;
                    }

                    case EAnimationEventParameter.Customization:
                    {
                        var action = (Action<CustomAnimationEventParameter>)method.CreateDelegate(typeof(Action<CustomAnimationEventParameter>), mono);
                        _eventContainer.RegisterEventAction(eventHash, new EventAction<CustomAnimationEventParameter>(action, paramType));
                        break;
                    }
                }
            });
        }


        public void ReceiveEvent(int eventHash, AnimationEventParameter animationEventParameter)
        {
            bool succeed = _eventContainer.Invoke(eventHash, animationEventParameter, out string errorMessage);

            if (debug && succeed == false)
            {
                Debug.Log($"<color=magenta>[Animation Event Receiver]</color> {errorMessage}");
            }
        }


        public void AddEvent<T>(string eventName, int targetLayer, AnimatorStateInfo stateInfo, AnimationEventParameter param, Action<T> action)
        {
            
        }
        
        
        public void AddEvent(string eventName, int targetLayer, AnimatorStateInfo stateInfo, Action action)
        {
            
        }


        public void RemoveEvent(string eventName, int targetLayer)
        {
            
        }
    }
}