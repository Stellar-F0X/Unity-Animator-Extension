using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AnimatorExtension.Parameters;

namespace AnimatorExtension
{
    internal class StateEventBehaviour : StateMachineBehaviour
    {
        public List<AnimationEvent> animationEventList = new List<AnimationEvent>();

        private bool _initialized;

        private int _loopCount;

        private AnimationEventController _controller;

        private ILookup<EEventDispatchType, AnimationEvent> _onRepeatingEvent;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _loopCount = 1;

            if (animationEventList.Count == 0)
            {
                return;
            }

            if (_initialized == false)
            {
                _initialized = true;

                bool foundReceiver = animator.TryGetComponent(out _controller);

                Debug.Assert(foundReceiver, "There are no Receivers on this object or its hierarchy.");

                _onRepeatingEvent = animationEventList
                    .Where(e => e.dispatchType is EEventDispatchType.Start or EEventDispatchType.End)
                    .ToLookup(key => key.dispatchType, result => result);
            }

            for (int i = 0; i < animationEventList.Count; i++)
            {
                if (animationEventList[i].dispatchType is EEventDispatchType.Enter or EEventDispatchType.Start)
                {
                    AnimationEvent animEvent = animationEventList[i];

                    this.InitializeCustomizationParameter(animationEventList[i].parameter, animator, layerIndex);
                    this._controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                    this.animationEventList[i].hasTriggered = true;
                }
            }
        }



        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animationEventList.Count == 0)
            {
                return;
            }

            if (this.IsAnimationRepeated(stateInfo))
            {
                _onRepeatingEvent.ForEach(EEventDispatchType.End, animEvent =>
                {
                    this.InitializeCustomizationParameter(animEvent.parameter, animator, layerIndex);
                    _controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                });

                _onRepeatingEvent.ForEach(EEventDispatchType.Start, animEvent =>
                {
                    this.InitializeCustomizationParameter(animEvent.parameter, animator, layerIndex);
                    _controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                });

                animationEventList.ForEach(e => e.hasTriggered = false);
            }

            float currentTime = stateInfo.normalizedTime % 1f;

            for (int i = 0; i < animationEventList.Count; i++)
            {
                if (animationEventList[i].dispatchType != EEventDispatchType.None)
                {
                    //반복적인 이벤트 전송을 이 함수에서 처리함
                    this.NotifyEvents(animationEventList[i], currentTime, animator, layerIndex);
                }
            }
        }



        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animationEventList.Count == 0)
            {
                return;
            }

            for (int i = 0; i < animationEventList.Count; i++)
            {
                if (animationEventList[i].dispatchType is EEventDispatchType.Exit or EEventDispatchType.End)
                {
                    AnimationEvent animEvent = animationEventList[i];

                    this.InitializeCustomizationParameter(animationEventList[i].parameter, animator, layerIndex);
                    this._controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                    this.animationEventList[i].hasTriggered = true;
                }
            }
        }



        private bool IsAnimationRepeated(AnimatorStateInfo stateInfo)
        {
            if ((int)stateInfo.normalizedTime >= _loopCount)
            {
                _loopCount++;

                return true;
            }

            return false;
        }



        private void InitializeCustomizationParameter(AnimationEventParameter param, Animator animator, int layerIndex)
        {
            if (param.parameterType != EAnimationEventParameter.Customization)
            {
                return;
            }

            param.customValue.layerIndex = layerIndex;

            param.customValue.stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

            param.customValue.clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        }



        private void NotifyEvents(AnimationEvent animEvent, float currentTime, Animator animator, int layerIndex)
        {
            switch (animEvent.dispatchType)
            {
                case EEventDispatchType.Update:
                {
                    this.InitializeCustomizationParameter(animEvent.parameter, animator, layerIndex);
                    _controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                    animEvent.hasTriggered = true;
                    break;
                }

                case EEventDispatchType.Range:
                {
                    if (currentTime >= animEvent.rangeTriggerTime.min)
                    {
                        if (currentTime <= animEvent.rangeTriggerTime.max)
                        {
                            this.InitializeCustomizationParameter(animEvent.parameter, animator, layerIndex);
                            _controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                            break;
                        }

                        animEvent.hasTriggered = true;
                    }

                    break;
                }

                case EEventDispatchType.Point:
                {
                    if (animEvent.hasTriggered == false && currentTime > animEvent.triggerTime)
                    {
                        this.InitializeCustomizationParameter(animEvent.parameter, animator, layerIndex);
                        _controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                        animEvent.hasTriggered = true;
                    }

                    break;
                }
            }
        }
    }
}