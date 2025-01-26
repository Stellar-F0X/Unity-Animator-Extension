using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension
{
    public abstract class EventCallback : ISerializationCallbackReceiver
    {
        public EventCallback(EAnimationEventParameter animationEventParameter)
        {
            this.animationEventParameter = animationEventParameter;
        }
        
        public readonly EAnimationEventParameter animationEventParameter;
        
        
        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}