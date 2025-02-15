using System;
using UnityEngine;

namespace AnimatorExtension.Parameters
{
    [Serializable]
    public abstract class CustomAnimationEventParameter
    {
        public int layerIndex { get; set; }
        
        public AnimatorStateInfo stateInfo { get; set; }
        
        public virtual void OnBeforeEventTrigger() { }
        public virtual void OnAfterEventTrigger() { }
    }
}