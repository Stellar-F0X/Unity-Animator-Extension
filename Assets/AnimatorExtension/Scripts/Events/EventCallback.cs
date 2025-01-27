using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension
{
    public abstract class EventCallback
    {
        public EventCallback(EAnimationEventParameter animationEventParameter)
        {
            this.animationEventParameter = animationEventParameter;
        }
        
        public readonly EAnimationEventParameter animationEventParameter;
    }
}