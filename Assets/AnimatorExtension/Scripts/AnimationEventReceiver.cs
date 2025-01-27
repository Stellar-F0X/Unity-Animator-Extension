using System;
using System.Reflection;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationEventReceiver : MonoBehaviour
    {
        public bool debug = false;

        private readonly static Type _SearchAttributeType = typeof(AnimationEventAttribute);

        private readonly EventContainer _eventContainer = new EventContainer();

        private Animator _animator;

        private const BindingFlags _EVENT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


        public Animator animator
        {
            get { return _animator == null ? _animator = GetComponent<Animator>() : _animator; }
        }


        private void Awake()
        {
            MonoBehaviour[] components = GetComponentsInChildren<MonoBehaviour>(true);

            foreach (MonoBehaviour component in components)
            {
                Type type = component.GetType();

                foreach (MethodInfo method in type.GetMethods(_EVENT_BINDING_FLAGS))
                {
                    if (method.GetCustomAttribute(_SearchAttributeType) is AnimationEventAttribute attribute)
                    {
                        int eventHash = Extension.StringToHash(attribute.eventName);
                        
                        switch (attribute.eventParameter)
                        {
                            case EAnimationEventParameter.Void:
                            {
                                Action action = (Action)method.CreateDelegate(typeof(Action), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Int:
                            {
                                Action<int> action = (Action<int>)method.CreateDelegate(typeof(Action<int>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<int>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Float:
                            {
                                Action<float> action = (Action<float>)method.CreateDelegate(typeof(Action<float>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<float>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Bool:
                            {
                                Action<bool> action = (Action<bool>)method.CreateDelegate(typeof(Action<bool>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<bool>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Tag:
                            case EAnimationEventParameter.String:
                            {
                                Action<string> action = (Action<string>)method.CreateDelegate(typeof(Action<string>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<string>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Color:
                            {
                                Action<Color> action = (Action<Color>)method.CreateDelegate(typeof(Action<Color>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<Color>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.LayerMask:
                            {
                                Action<LayerMask> action = (Action<LayerMask>)method.CreateDelegate(typeof(Action<LayerMask>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<LayerMask>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Vector2:
                            {
                                Action<Vector2> action = (Action<Vector2>)method.CreateDelegate(typeof(Action<Vector2>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<Vector2>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Vector3:
                            {
                                Action<Vector3> action = (Action<Vector3>)method.CreateDelegate(typeof(Action<Vector3>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<Vector3>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Quaternion:
                            {
                                Action<Quaternion> action = (Action<Quaternion>)method.CreateDelegate(typeof(Action<Quaternion>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<Quaternion>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.GameObject:
                            {
                                Action<GameObject> action = (Action<GameObject>)method.CreateDelegate(typeof(Action<GameObject>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<GameObject>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.ScriptableObject:
                            {
                                Action<ScriptableObject> action = (Action<ScriptableObject>)method.CreateDelegate(typeof(Action<ScriptableObject>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<ScriptableObject>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.AnimationCurve:
                            {
                                Action<AnimationCurve> action = (Action<AnimationCurve>)method.CreateDelegate(typeof(Action<AnimationCurve>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<AnimationCurve>(action, attribute.eventParameter));
                                break;
                            }

                            case EAnimationEventParameter.Customization:
                            {
                                //코드 생성기를 써보자.
                                var action = (Action<CustomAnimationEventParameter>)method.CreateDelegate(typeof(Action<CustomAnimationEventParameter>), component);
                                _eventContainer.RegisterEventAction(eventHash, new EventAction<CustomAnimationEventParameter>(action, attribute.eventParameter));
                                break;
                            }
                        }
                    }
                }
            }
        }


        private void RegisterEvent(int eventHash, EAnimationEventParameter eventParameter, MethodInfo method, Component component)
        {

        }


        public void ReceiveEvent(int eventHash, AnimationEventParameter animationEventParameter)
        {
            bool succeed = _eventContainer.Invoke(eventHash, animationEventParameter, out string errorMessage);

            if (debug && succeed == false)
            {
                Debug.Log($"<color=magenta>[Animation Event Receiver]</color> {errorMessage}");
            }
        }
    }
}