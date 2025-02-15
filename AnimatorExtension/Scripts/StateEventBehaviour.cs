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
            if (animationEventList.Count == 0)
            {
                return;
            }

            _loopCount = 1;

            if (_initialized == false)
            {
                _initialized = animator.TryGetComponent(out _controller);
                
                Debug.Assert(_initialized, "There are no Receivers on this object or its hierarchy.");
                
                if (_initialized == false)
                {
                    return;
                }

                _onRepeatingEvent = animationEventList
                    .Where(e => e.dispatchType is EEventDispatchType.Start or EEventDispatchType.End)
                    .ToLookup(key => key.dispatchType, result => result);
            }


            for (int i = 0; i < animationEventList.Count; i++)
            {
                if (animationEventList[i].dispatchType is EEventDispatchType.Enter or EEventDispatchType.Start)
                {
                    AnimationEvent animEvent = animationEventList[i];

                    this.InitializeParameter(ref animEvent.parameter, stateInfo, layerIndex);
                    
                    this._controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                    this.animationEventList[i].hasTriggered = true;
                }
            }
        }



        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_initialized == false || animationEventList.Count == 0)
            {
                return;
            }

            if (this.IsAnimationRepeated(stateInfo))
            {
                _onRepeatingEvent.ForEach(EEventDispatchType.End, animEvent =>
                {
                    this.InitializeParameter(ref animEvent.parameter, stateInfo, layerIndex);
                    _controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                });

                _onRepeatingEvent.ForEach(EEventDispatchType.Start, animEvent =>
                {
                    this.InitializeParameter(ref animEvent.parameter, stateInfo, layerIndex);
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
                    this.NotifyEvents(animationEventList[i], currentTime, stateInfo, layerIndex);
                }
            }
        }



        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_initialized == false || animationEventList.Count == 0)
            {
                return;
            }

            for (int i = 0; i < animationEventList.Count; i++)
            {
                if (animationEventList[i].dispatchType is EEventDispatchType.Exit or EEventDispatchType.End)
                {
                    AnimationEvent animEvent = animationEventList[i];

                    this.InitializeParameter(ref animEvent.parameter, stateInfo, layerIndex);
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



        private void InitializeParameter(ref AnimationEventParameter param, AnimatorStateInfo info, int layerIndex)
        {
            switch (param.parameterType)
            {
                case EAnimationEventParameter.Customization:
                {
                    param.customValue.stateInfo = info;
                    param.customValue.layerIndex = layerIndex;
                    break;
                }

                case EAnimationEventParameter.AnimatorInfo:
                {
                    param.animationInfoValue.isTransitioning = _controller.animator.IsInTransition(layerIndex);
                    param.animationInfoValue.nodeName = _controller.animator.ResolveHash(info.shortNameHash);
                    param.animationInfoValue.nodeTag = _controller.animator.ResolveHash(info.tagHash);
                    param.animationInfoValue.layerIndex = layerIndex;
                    param.animationInfoValue.stateInfo = info;
                    break;
                }
            }
        }



        private void NotifyEvents(AnimationEvent animEvent, float currentTime, AnimatorStateInfo stateInfo, int layerIndex)
        {
            switch (animEvent.dispatchType)
            {
                case EEventDispatchType.Update:
                {
                    this.InitializeParameter(ref animEvent.parameter, stateInfo, layerIndex);
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
                            this.InitializeParameter(ref animEvent.parameter, stateInfo, layerIndex);
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
                        this.InitializeParameter(ref animEvent.parameter, stateInfo, layerIndex);
                        _controller.ReceiveEvent(animEvent.eventName, animEvent.eventHash, animEvent.parameter);
                        animEvent.hasTriggered = true;
                    }

                    break;
                }
            }
        }
    }
}