using System;
using System.Linq;
using UnityEngine;

namespace AnimatorExtension.Parameters
{
    [Serializable]
    public struct AnimatorInfo
    {
        public int layerIndex;
        public string nodeName;
        public string nodeTag;
        
        public AnimatorStateInfo stateInfo;
        public AnimatorTransitionInfo transitionInfo;
        public AnimatorClipInfo[] clipInfos;
    }
}