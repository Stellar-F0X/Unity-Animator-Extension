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
            get { return _animator ?? TryGetComponent(out _animator) ? _animator : null; }
        }


        private void Awake()
        {
            MonoBehaviour[] components = GetComponentsInChildren<MonoBehaviour>(true);

            foreach (MonoBehaviour component in components)
            {
                Type type = component.GetType();

                foreach (MethodInfo methodInfo in type.GetMethods(_EVENT_BINDING_FLAGS))
                {
                    if (methodInfo.GetCustomAttribute(_SearchAttributeType) is AnimationEventAttribute attribute)
                    {
                        int eventHash = Extension.StringToHash(attribute.eventName);
                        EAnimationEventParameter animationEventParameter = attribute.animationEventParameter;
                        this.RegisterEvent(eventHash, animationEventParameter, methodInfo, component);
                    }
                }
            }
        }


        private void RegisterEvent(int eventHash, EAnimationEventParameter animationEventParameter, MethodInfo method, Component component)
        {
            switch (animationEventParameter)
            {
                case EAnimationEventParameter.Void:
                    Action voidAction = (Action)method.CreateDelegate(typeof(Action), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction(voidAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.Int:
                    Action<int> intAction = (Action<int>)method.CreateDelegate(typeof(Action<int>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<int>(intAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.Float:
                    Action<float> floatAction = (Action<float>)method.CreateDelegate(typeof(Action<float>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<float>(floatAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.Bool:
                    Action<bool> boolAction = (Action<bool>)method.CreateDelegate(typeof(Action<bool>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<bool>(boolAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.String:
                    Action<string> stringAction = (Action<string>)method.CreateDelegate(typeof(Action<string>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<string>(stringAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.Vector3:
                    Action<Vector3> vectorAction = (Action<Vector3>)method.CreateDelegate(typeof(Action<Vector3>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<Vector3>(vectorAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.Quaternion:
                    Action<Quaternion> quaternionAction = (Action<Quaternion>)method.CreateDelegate(typeof(Action<Quaternion>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<Quaternion>(quaternionAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.GameObject:
                    Action<GameObject> gameObjectAction = (Action<GameObject>)method.CreateDelegate(typeof(Action<GameObject>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<GameObject>(gameObjectAction, animationEventParameter));
                    break;

                case EAnimationEventParameter.Color:
                    Action<Color> colorAction = (Action<Color>)method.CreateDelegate(typeof(Action<Color>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<Color>(colorAction, animationEventParameter));
                    break;
            }
        }


        public void ReceiveEvent(int eventHash, AnimationEventParameter animationEventParameter)
        {
            bool succeed = _eventContainer.Invoke(eventHash, animationEventParameter, out string errorMessage);

            if (debug && !succeed)
            {
                Debug.Log($"<color=magenta>[Animation Event Receiver]</color> {errorMessage}");
            }
        }
    }
}