using System;
using UnityEngine;

namespace AnimatorExtension.Parameters
{
    [Serializable]
    public struct AnimationInfo
    {
        public int layerIndex;
        public string nodeName;
        public string nodeTag;
        public bool isTransitioning;
        
        public AnimatorStateInfo stateInfo;
    }
}