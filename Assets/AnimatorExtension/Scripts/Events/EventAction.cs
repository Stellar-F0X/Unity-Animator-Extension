using System;
using AnimatorExpansion.Parameters;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatorExpansion
{
    public class EventAction<T> : EventCallback
    {
        public EventAction(Action<T> action, EParameterType parameterType) : base(parameterType)
        {
            this.action = action;
        }
        
        public Action<T> action;

        public void Invoke(T parameter)
        {
            action?.Invoke(parameter);
        }
    }

    
    public class EventAction : EventCallback
    {
        public EventAction(Action action, EParameterType parameterType) : base(parameterType)
        {
            this.action = action;
        }
        
        public Action action;
        
        public void Invoke()
        {
            action?.Invoke();
        }
    }
}