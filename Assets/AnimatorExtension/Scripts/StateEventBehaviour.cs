using UnityEngine;
using System.Collections.Generic;
using AnimatorExpansion.Parameters;

namespace AnimatorExpansion
{
    internal class StateEventBehaviour : StateMachineBehaviour
    {
        public List<AnimationEvent> animationEventList = new List<AnimationEvent>();

        private bool _initialized;
        
        private int _loopCount;

        private AnimationEventReceiver _receiver;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _loopCount = 1;
            
            if (_initialized == false)
            {
                _initialized = true;

                bool foundReceiver = animator.TryGetComponent(out _receiver);

                Debug.Assert(foundReceiver, "There are no Receivers on this object or its hierarchy.");
            }

            for (int i = 0; i < animationEventList.Count; i++)
            {
                var animationEvent = animationEventList[i];
                
                if (animationEvent.sendType != EEventSendType.Enter)
                {
                    continue;
                }

                _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                animationEvent.hasTriggered = true;
            }
        }



        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animationEventList.Count == 0)
            {
                return;
            }
            
            this.TryInitializeEvent(stateInfo);

            float currentTime = stateInfo.normalizedTime % 1f;

            for (int i = 0; i < animationEventList.Count; i++)
            {
                var animationEvent = animationEventList[i];
                
                if (animationEvent.sendType == EEventSendType.None)
                {
                    continue;
                }

                this.NotifyEvents(animationEvent, currentTime);
            }
        }



        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            for (int i = 0; i < animationEventList.Count; i++)
            {
                var animationEvent = animationEventList[i];
                
                if (animationEvent.sendType == EEventSendType.Exit)
                {
                    _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                    animationEvent.hasTriggered = true;
                }
            }
        }
        
        
        
        
        private void TryInitializeEvent(AnimatorStateInfo stateInfo)
        {
            if ((int)stateInfo.normalizedTime >= _loopCount)
            {
                _loopCount++;

                for (int i = 0; i < animationEventList.Count; i++)
                {
                    animationEventList[i].hasTriggered = false;
                }
            }
        }
        
        
        
        private void NotifyEvents(AnimationEvent animationEvent, float currentTime)
        {
            if (animationEvent.sendType == EEventSendType.Update)
            {
                _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                
                animationEvent.hasTriggered = true;
            }
            else if (animationEvent.sendType == EEventSendType.Range)
            {
                MinMax rangeTriggerTime = animationEvent.rangeTriggerTime;

                if (currentTime >= rangeTriggerTime.min)
                {
                    if (currentTime <= rangeTriggerTime.max)
                    {
                        _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                    }
                    else
                    {
                        animationEvent.hasTriggered = true;
                    }
                }
            }
            else if (animationEvent.sendType == EEventSendType.Point)
            {
                if (animationEvent.hasTriggered)
                {
                    return;
                }
                
                if (currentTime > animationEvent.triggerTime)
                {
                    _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                    
                    animationEvent.hasTriggered = true;
                }
            }
        }
    }
}