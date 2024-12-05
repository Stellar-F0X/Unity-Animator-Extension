using System;
using AnimatorExpansion;
using UnityEngine;

namespace AnimatorExpansion
{
    public class AnimationEventSender : StateMachineBehaviour
    {
        public string eventName;

        [Range(0f, 1f)] public float triggerTime;

        private int _eventHash;
        private bool _hasTriggered;


        private void Awake()
        {
            _eventHash = Utility.StringToHash(eventName);
        }


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _hasTriggered = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentTime = stateInfo.normalizedTime % 1f;

            if (!_hasTriggered && currentTime >= triggerTime)
            {
                this.NotifyReceiver(animator);
                this._hasTriggered = true;
            }
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }


        private void NotifyReceiver(Animator animator)
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

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}