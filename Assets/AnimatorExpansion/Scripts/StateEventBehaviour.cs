using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorExpansion
{
    public class StateEventBehaviour : StateMachineBehaviour
    {
        public string eventName;

        [Range(0f, 1f)] 
        public float triggerTime;

        private int _eventHash;
        private bool _hasTriggered;

        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _hasTriggered = false;
        }

        
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentTime = stateInfo.normalizedTime % 1f;

            if (!_hasTriggered && currentTime >= triggerTime)
            {
                this.NotifyEventToReceiver(animator);
                this._hasTriggered = true;
            }
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }


        private void NotifyEventToReceiver(Animator animator)
        {
            if (animator.TryGetComponent<AnimationEventReceiver>(out var receiver))
            {
                receiver.TriggerEvent(_eventHash);
            }
            else
            {
                Debug.LogError("No Animation Event Receiver");
            }
        }
    }
}