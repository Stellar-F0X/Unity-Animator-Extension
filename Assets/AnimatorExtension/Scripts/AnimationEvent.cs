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
        public bool hasTriggered;
        
        public AnimationEventParameter parameter;
        public EEventDispatchType dispatchType;
        public MinMax rangeTriggerTime;
    }
}