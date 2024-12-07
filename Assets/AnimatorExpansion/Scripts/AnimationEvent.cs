using System;
using AnimatorExpansion.Parameters;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatorExpansion
{
    [Serializable]
    public class AnimationEvent
    {
        public string eventName;
        public int eventHash; 
        public float triggerTime;
        public SEventParameter parameter;
        public bool hasTriggered;
    }
}