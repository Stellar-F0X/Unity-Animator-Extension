using System.Collections.Generic;
using AnimatorExpansion.Parameters;
using UnityEngine;

namespace AnimatorExpansion
{
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationEventReceiver : MonoBehaviour
    {
        public Dictionary<int, SEventReceiveInfo> eventReceiveInfoList = new Dictionary<int, SEventReceiveInfo>();
        
        
        private void Awake()
        {
            
        }


        public void ReceiveEvent(int eventHash, SEventParameter sEventParameter)
        {
            
        }
    }
}