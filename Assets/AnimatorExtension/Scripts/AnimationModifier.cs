using UnityEngine;

namespace AnimatorExtension
{
    public class AnimationModifier : StateMachineBehaviour
    {
        public string speedMultiplierParameterName; 
        public AnimationSpeedInfo[] speedInfos;

        private int _speedInfoIndex = 0;
        private int _loopCount = 0;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _speedInfoIndex = 0;
        }

        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_speedInfoIndex >= speedInfos.Length)
            {
                return;
            }
            
            this.TryInitializeEvent(stateInfo);
            
            float currentTime = stateInfo.normalizedTime % 1f;
            MinMax controlRange = speedInfos[_speedInfoIndex].controlRange;

            if (controlRange.min <= currentTime)
            {
                if (currentTime <= controlRange.max)
                {
                    float normalValue = speedInfos[_speedInfoIndex].speedCurve.Evaluate(currentTime);
                    animator.SetFloat(speedMultiplierParameterName, normalValue);
                }
                else if (_speedInfoIndex < speedInfos.Length)
                {
                    _speedInfoIndex++;
                }
            }
        }

        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
        
        
        private void TryInitializeEvent(AnimatorStateInfo stateInfo)
        {
            if ((int)stateInfo.normalizedTime >= _loopCount)    
            {
                _loopCount++;
                _speedInfoIndex = 0;
            }
        }
    }
}
