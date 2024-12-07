using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimatorExpansion
{
    public sealed class AnimationEventReceiver : MonoBehaviour
    {
        [SerializeField]
        private List<AnimationEvent> _animationEvents = new List<AnimationEvent>();


        private void Awake()
        {
            
        }


        public void TriggerEvent(int eventHash)
        {
            var animationEvent = _animationEvents.FirstOrDefault(se => se.eventHash == eventHash);

            if (animationEvent != null)
            {
                animationEvent.animationEvent?.Invoke();
            }
            else
            {
                Debug.LogError("not");
            }
        }
    }
}