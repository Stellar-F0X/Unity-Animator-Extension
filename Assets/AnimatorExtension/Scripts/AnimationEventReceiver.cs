using System;
using System.Reflection;
using AnimatorExtension.Parameters;
using UnityEditor.Animations;
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

        private Animator _cachedAnimator;

        private const BindingFlags _EVENT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


        public Animator animator
        {
            get { return _cachedAnimator ??= GetComponent<Animator>(); }
        }

        public AnimatorController controller
        {
            get { return animator.runtimeAnimatorController as AnimatorController; }
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
                        EParameterType parameter = attribute.parameterType;
                        this.RegisterEvent(eventHash, parameter, methodInfo, component);
                    }
                }
            }
        }


        private void RegisterEvent(int eventHash, EParameterType parameter, MethodInfo method, Component component)
        {
            switch (parameter)
            {
                case EParameterType.Void:
                    Action voidAction = (Action)method.CreateDelegate(typeof(Action), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction(voidAction, parameter));
                    break;

                case EParameterType.Int:
                    Action<int> intAction = (Action<int>)method.CreateDelegate(typeof(Action<int>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<int>(intAction, parameter));
                    break;

                case EParameterType.Float:
                    Action<float> floatAction = (Action<float>)method.CreateDelegate(typeof(Action<float>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<float>(floatAction, parameter));
                    break;

                case EParameterType.Bool:
                    Action<bool> boolAction = (Action<bool>)method.CreateDelegate(typeof(Action<bool>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<bool>(boolAction, parameter));
                    break;

                case EParameterType.String:
                    Action<string> stringAction = (Action<string>)method.CreateDelegate(typeof(Action<string>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<string>(stringAction, parameter));
                    break;

                case EParameterType.Vector3:
                    Action<Vector3> vectorAction = (Action<Vector3>)method.CreateDelegate(typeof(Action<Vector3>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<Vector3>(vectorAction, parameter));
                    break;

                case EParameterType.Quaternion:
                    Action<Quaternion> quaternionAction = (Action<Quaternion>)method.CreateDelegate(typeof(Action<Quaternion>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<Quaternion>(quaternionAction, parameter));
                    break;

                case EParameterType.GameObject:
                    Action<GameObject> gameObjectAction = (Action<GameObject>)method.CreateDelegate(typeof(Action<GameObject>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<GameObject>(gameObjectAction, parameter));
                    break;

                case EParameterType.Color:
                    Action<Color> colorAction = (Action<Color>)method.CreateDelegate(typeof(Action<Color>), component);
                    _eventContainer.RegisterEventAction(eventHash, new EventAction<Color>(colorAction, parameter));
                    break;
            }
        }


        public void ReceiveEvent(int eventHash, SEventParameter sEventParameter)
        {
            bool succeed = _eventContainer.Invoke(eventHash, sEventParameter, out string errorMessage);

            if (debug && !succeed)
            {
                Debug.Log($"<color=magenta>[Animation Event Receiver]</color> {errorMessage}");
            }
        }
    }
}