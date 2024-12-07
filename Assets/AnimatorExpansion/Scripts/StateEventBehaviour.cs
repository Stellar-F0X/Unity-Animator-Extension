using System;
using System.Collections.Generic;
using AnimatorExpansion.Parameters;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AnimatorExpansion
{
    public class StateEventBehaviour : StateMachineBehaviour
    {
        public List<AnimationEvent> animationEventList = new List<AnimationEvent>();
        
        private AnimationEventReceiver _receiver;
        
        [SerializeField]
        private bool _initialized;
        
        [SerializeField]
        private ESearchType _receiverSearchType;

        
        
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_initialized == false)
            {
                _initialized = true;
                
                switch (_receiverSearchType)
                {
                    case ESearchType.Parent: _receiver = animator.GetComponentInParent<AnimationEventReceiver>(); break;
                    
                    case ESearchType.Sibling: _receiver = animator.GetComponent<AnimationEventReceiver>(); break;
                    
                    case ESearchType.Child: _receiver = animator.GetComponentInChildren<AnimationEventReceiver>(); break;
                }

                Debug.Assert(_receiver != null, "There are no Receivers on this object or its hierarchy.");
            }
            
            foreach (var animationEvent in animationEventList)
            {
                animationEvent.hasTriggered = false;
            }
        }

        
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentTime = stateInfo.normalizedTime % 1f;

            foreach (var animationEvent in animationEventList)
            {
                if (!animationEvent.hasTriggered && currentTime >= animationEvent.triggerTime)
                {
                    if (animationEvent.parameter.hasParameter)
                    {
                        _receiver.TriggerEvent(animationEvent.eventHash, animationEvent.parameter);

                        animationEvent.hasTriggered = true;
                    }
                    else
                    {
                        Debug.LogError("There are no parameters to pass from this event.");
                    }
                }
            }
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
    }
}