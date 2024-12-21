using System;
using AnimatorExtension.Parameters;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AnimatorExtension
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