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
    internal class StateEventBehaviour : StateMachineBehaviour
    {
        public List<AnimationEvent> animationEventList = new List<AnimationEvent>();

        private bool _initialized;
        
        private AnimationEventReceiver _receiver;
        


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_initialized == false)
            {
                _initialized = true;

                bool foundReceiver = animator.TryGetComponent(out _receiver);

                Debug.Assert(foundReceiver, "There are no Receivers on this object or its hierarchy.");
            }

            foreach (var animationEvent in animationEventList)
            {
                if (animationEvent.sendType == EEventSendType.Enter)
                {
                    _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                    animationEvent.hasTriggered = true;
                }
                else
                {
                    animationEvent.hasTriggered = false;
                }
            }
        }



        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animationEventList.Count == 0)
            {
                return;
            }

            float currentTime = stateInfo.normalizedTime % 1f;

            foreach (var animationEvent in animationEventList)
            {
                if (animationEvent.hasTriggered || animationEvent.sendType == EEventSendType.None)
                {
                    continue;
                }

                if (animationEvent.sendType == EEventSendType.Update)
                {
                    _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                    animationEvent.hasTriggered = true;
                }
                else if (animationEvent.sendType == EEventSendType.Point)
                {
                    if (currentTime >= animationEvent.triggerTime)
                    {
                        _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                        animationEvent.hasTriggered = true;
                    }
                }
                else if (animationEvent.sendType == EEventSendType.Range)
                {
                    MinMax rangeTriggerTime = animationEvent.rangeTriggerTime;

                    if (currentTime >= rangeTriggerTime.min && currentTime <= rangeTriggerTime.max)
                    {
                        _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                    }
                    else if (currentTime > rangeTriggerTime.max)
                    {
                        animationEvent.hasTriggered = true;
                    }
                }
            }
        }

        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var animationEvent in animationEventList)
            {
                if (animationEvent.sendType == EEventSendType.Exit)
                {
                    _receiver.ReceiveEvent(animationEvent.eventHash, animationEvent.parameter);
                    animationEvent.hasTriggered = true;
                }
            }
        }
    }
}