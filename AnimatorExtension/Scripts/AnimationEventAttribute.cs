using System;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AnimationEventAttribute : PropertyAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventParameter"></param>
        public AnimationEventAttribute(string eventName, EAnimationEventParameter eventParameter = EAnimationEventParameter.Void)
        {
            this.eventName = eventName;
            this.eventParameter = eventParameter;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventParameter"></param>
        /// <param name="customParam"></param>
        public AnimationEventAttribute(string eventName, EAnimationEventParameter eventParameter, Type customParam)
        {
            this.eventName = eventName;
            this.eventParameter = eventParameter;
            this.customParameterType = customParam;
        }

        public string eventName;
        public Type customParameterType;
        public EAnimationEventParameter eventParameter;
    }
}