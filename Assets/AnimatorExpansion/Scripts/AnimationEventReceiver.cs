using System;
using System.Collections.Generic;
using System.Linq;
using AnimatorExpansion.Parameters;
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


        public void TriggerEvent(int eventHash, SEventParameter sEventParameter)
        {
            
        }
    }
}