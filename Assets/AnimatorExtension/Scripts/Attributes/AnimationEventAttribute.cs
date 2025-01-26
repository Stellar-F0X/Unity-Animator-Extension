using System;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AnimationEventAttribute : PropertyAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="animationEventParameter"></param>
        public AnimationEventAttribute(string eventName, EAnimationEventParameter animationEventParameter = EAnimationEventParameter.Void)
        {
            this.eventName = eventName;
            this.animationEventParameter = animationEventParameter;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="animationEventParameter"></param>
        /// <param name="customParam"></param>
        public AnimationEventAttribute(string eventName, EAnimationEventParameter animationEventParameter, Type customParam)
        {
            this.eventName = eventName;
            this.animationEventParameter = animationEventParameter;
            this.customParameterType = customParam;
        }

        public string eventName;
        public Type customParameterType;
        public EAnimationEventParameter animationEventParameter;
    }
}