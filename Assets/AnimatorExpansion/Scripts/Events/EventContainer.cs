using System;
using System.Collections.Generic;
using AnimatorExpansion.Parameters;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatorExpansion
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
        
        
        public bool Invoke(int eventHash, SEventParameter parameter, out string errorMessage)
        {
            if (eventList.TryGetValue(eventHash, out var callback) == false)
            {
                errorMessage = "Event not found";
                return false;
            }

            if (callback.parameterType != parameter.parameterType)
            {
                errorMessage = $" Event parameter type mismatch. {callback.parameterType} != {parameter.parameterType}";
                return false;
            }

            switch (parameter.parameterType)
            {
                case EParameterType.Void: GetEventAction(callback).Invoke(); break;

                case EParameterType.Int: GetEventAction<int>(callback).Invoke(parameter.intValue); break;

                case EParameterType.Float: GetEventAction<float>(callback).Invoke(parameter.floatValue); break;

                case EParameterType.Bool: GetEventAction<bool>(callback).Invoke(parameter.boolValue); break;

                case EParameterType.Color: GetEventAction<Color>(callback).Invoke(parameter.colorValue); break;

                case EParameterType.String: GetEventAction<string>(callback).Invoke(parameter.stringValue); break;

                case EParameterType.Vector3: GetEventAction<Vector3>(callback).Invoke(parameter.vector3Value); break;

                case EParameterType.Quaternion: GetEventAction<Quaternion>(callback).Invoke(parameter.quaternionValue); break;
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