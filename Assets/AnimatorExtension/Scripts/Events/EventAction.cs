using System;
using AnimatorExtension.Parameters;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatorExtension
{
    public class EventAction<T> : EventCallback
    {
        public EventAction(Action<T> action, EAnimationEventParameter animationEventParameter) : base(animationEventParameter)
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
        public EventAction(Action action, EAnimationEventParameter animationEventParameter) : base(animationEventParameter)
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