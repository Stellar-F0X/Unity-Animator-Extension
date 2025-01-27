using System;
using UnityEngine;

namespace AnimatorExtension.Parameters
{
    [Serializable]
    public abstract class CustomAnimationEventParameter
    {
        protected int _layerIndex;
        protected Animator _animator;
        protected AnimatorStateInfo _stateInfo;
        
        public abstract void OnBeforeEventTrigger();
    }
}