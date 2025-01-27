using System;
using System.Collections.Generic;
using AnimatorExtension.Parameters;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatorExtension
{
    public class EventContainer
    {
        public readonly Dictionary<int, EventCallback> eventList = new Dictionary<int, EventCallback>();

        
        public void RegisterEventAction(int eventHash, EventCallback callback)
        {
            if (eventList.ContainsKey(eventHash))
            {
                Debug.LogError("Event already exists");
                return;
            }
            
            eventList.Add(eventHash, callback);
        }
        
        
        public bool Invoke(int eventHash, AnimationEventParameter parameter, out string errorMessage)
        {
            if (eventList.TryGetValue(eventHash, out var callback) == false)
            {
                errorMessage = "Event not found";
                return false;
            }

            if (callback.animationEventParameter != parameter.parameterType)
            {
                errorMessage = $" Event parameter type mismatch. {callback.animationEventParameter} != {parameter.parameterType}";
                return false;
            }

            switch (parameter.parameterType)
            {
                case EAnimationEventParameter.Void: GetEventAction(callback).Invoke(); break;

                case EAnimationEventParameter.Int: GetEventAction<int>(callback).Invoke(parameter.intValue); break;

                case EAnimationEventParameter.Float: GetEventAction<float>(callback).Invoke(parameter.floatValue); break;

                case EAnimationEventParameter.Bool: GetEventAction<bool>(callback).Invoke(parameter.boolValue); break;

                case EAnimationEventParameter.Color: GetEventAction<Color>(callback).Invoke(parameter.colorValue); break;

                case EAnimationEventParameter.Tag:
                case EAnimationEventParameter.String: GetEventAction<string>(callback).Invoke(parameter.stringValue); break;
                
                case EAnimationEventParameter.LayerMask: GetEventAction<LayerMask>(callback).Invoke(parameter.intValue); break; 

                case EAnimationEventParameter.Vector2: GetEventAction<Vector2>(callback).Invoke(parameter.vector2Value); break;
                
                case EAnimationEventParameter.Vector3: GetEventAction<Vector3>(callback).Invoke(parameter.vector3Value); break;

                case EAnimationEventParameter.Quaternion: GetEventAction<Quaternion>(callback).Invoke(parameter.quaternionValue); break;
                
                case EAnimationEventParameter.AnimationCurve: GetEventAction<AnimationCurve>(callback).Invoke(parameter.curveValue); break;
                
                case EAnimationEventParameter.GameObject: GetEventAction<GameObject>(callback).Invoke(parameter.gobjValue); break;
                
                case EAnimationEventParameter.ScriptableObject: GetEventAction<ScriptableObject>(callback).Invoke(parameter.sobjValue); break;
                
                case EAnimationEventParameter.Customization: GetEventAction<CustomAnimationEventParameter>(callback).Invoke(parameter.customValue); break;
            }

            errorMessage = "";
            return true;
        }


        private Action<T> GetEventAction<T>(EventCallback callback)
        {
            Action<T> eventAction = (callback as EventAction<T>).action;

            if (eventAction == null)
            {
                Debug.LogError("EventAction<T> action is null");
            }
            else
            {
                return eventAction;
            }

            return null;
        }

        
        private Action GetEventAction(EventCallback callback)
        {
            Action eventAction = (callback as EventAction).action;

            if (eventAction == null)
            {
                Debug.LogError("EventAction<T> action is null");
            }
            else
            {
                return eventAction;
            }

            return null;
        }
    }
}