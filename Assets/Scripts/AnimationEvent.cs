using System;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatorExpansion
{
    [Serializable]
    public class AnimationEvent
    {
        public AnimationEvent(string eventName)
        {
            this.eventName = eventName;
            eventHash = Utility.StringToHash(eventName);
        }
        
        [HideInInspector]
        public int eventHash; 
        
        public string eventName;
        public UnityEvent animationEvent;
        

        public void Initialize(Animator animator)
        {
            
        }
    }
}