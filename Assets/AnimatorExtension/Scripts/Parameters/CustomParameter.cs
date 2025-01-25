using System;
using UnityEngine;

namespace AnimatorExtension.Parameters
{
    [Serializable]
    public abstract class CustomParameter
    {
        protected int _layerIndex;
        protected Animator _animator;
        protected AnimatorStateInfo _stateInfo;
        
        public abstract void Execute();
    }

    [Serializable]
    public class ExampleCustomParameter : CustomParameter
    {
        public string parameterName;
        public string parameterType;
        
        [Range(0f, 1f)]
        public float parameterValue;
        
        public override void Execute()
        {
            Debug.Log(parameterName + " " + parameterType);
        }
    }
}