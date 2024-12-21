using System;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AnimationEventAttribute : PropertyAttribute
    {
        public AnimationEventAttribute(string eventName, EParameterType parameterType = EParameterType.Void)
        {
            this.eventName = eventName;
            this.parameterType = parameterType;
        }
        
        public string eventName; 
        public EParameterType parameterType;
    }
}