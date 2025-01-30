using System;
using AnimatorExtension.Parameters;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AnimatorExtension
{
    public class AnimationEventCallback
    {
        public AnimationEventCallback(Delegate callback, bool isRuntime = false)
        {
            this.isRuntime = isRuntime;
            this._callbackAction = callback;
        }

        public readonly bool isRuntime;
        
        private readonly Delegate _callbackAction;


        public void Invoke(AnimationEventParameter parameter)
        {
            switch (parameter.parameterType)
            {
                case EAnimationEventParameter.Void: (_callbackAction as Action).Invoke(); break;

                case EAnimationEventParameter.Int: (_callbackAction as Action<int>).Invoke(parameter.intValue); break;

                case EAnimationEventParameter.Float: (_callbackAction as Action<float>).Invoke(parameter.floatValue); break;

                case EAnimationEventParameter.Bool: (_callbackAction as Action<bool>).Invoke(parameter.boolValue); break;

                case EAnimationEventParameter.Color: (_callbackAction as Action<Color>).Invoke(parameter.colorValue); break;

                case EAnimationEventParameter.Tag: (_callbackAction as Action<string>).Invoke(parameter.stringValue); break;

                case EAnimationEventParameter.String: (_callbackAction as Action<string>).Invoke(parameter.stringValue); break;

                case EAnimationEventParameter.LayerMask: (_callbackAction as Action<LayerMask>).Invoke(parameter.intValue); break;

                case EAnimationEventParameter.Vector2: (_callbackAction as Action<Vector2>).Invoke(parameter.vector2Value); break;

                case EAnimationEventParameter.Vector3: (_callbackAction as Action<Vector3>).Invoke(parameter.vector3Value); break;

                case EAnimationEventParameter.Quaternion: (_callbackAction as Action<Quaternion>).Invoke(parameter.quaternionValue); break;

                case EAnimationEventParameter.AnimationCurve: (_callbackAction as Action<AnimationCurve>).Invoke(parameter.curveValue); break;

                case EAnimationEventParameter.GameObject: (_callbackAction as Action<GameObject>).Invoke(parameter.gameobjectValue); break;

                case EAnimationEventParameter.Object: (_callbackAction as Action<Object>).Invoke(parameter.objectValue); break;
                
                case EAnimationEventParameter.AnimatorInfo: (_callbackAction as Action<AnimatorInfo>).Invoke(parameter.animatorInfoValue); break;

                case EAnimationEventParameter.Customization:
                    parameter.customValue.OnBeforeEventTrigger();
                    (_callbackAction as Action<CustomAnimationEventParameter>).Invoke(parameter.customValue);
                    parameter.customValue.OnAfterEventTrigger();
                    break;
            }
        }
    }
}