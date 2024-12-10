using System;
using AnimatorExpansion.Parameters;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AnimatorExpansion
{
    [Serializable]
    public class AnimationEvent
    {
        public string eventName;
        public int eventHash; 
        public float triggerTime;
        public SEventParameter parameter;
        public EEventSendType sendType;
        public MinMax rangeTriggerTime;
        
        public bool hasTriggered;
    }
}