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
        /// <param name="parameterType"></param>
        public AnimationEventAttribute(string eventName, EParameterType parameterType = EParameterType.Void)
        {
            this.eventName = eventName;
            this.parameterType = parameterType;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="parameterType"></param>
        /// <param name="customParam"></param>
        public AnimationEventAttribute(string eventName, EParameterType parameterType, Type customParam)
        {
            this.eventName = eventName;
            this.parameterType = parameterType;
            this.customParameterType = customParam;
        }

        public string eventName;
        public Type customParameterType;
        public EParameterType parameterType;
    }
}