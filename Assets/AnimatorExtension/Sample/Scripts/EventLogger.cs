using System;
using AnimatorExtension;
using AnimatorExtension.Parameters;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatorExtension.Sample
{
    public class EventLogger : MonoBehaviour
    {
        private Animator animator;

        public string log0Message;
        public string log1Message;
        public string log2Message;

        

        [AnimationEvent("Log", EAnimationEventParameter.Void)]
        public void Print()
        {
            Debug.Log(log0Message);
        }


        [AnimationEvent("LogMessage", EAnimationEventParameter.Int)]
        public void LogIntMessage(int a)
        {
            Debug.Log(a);
        }


        [AnimationEvent("GetQuaternion", EAnimationEventParameter.Quaternion)]
        public void GetQuaternion(Quaternion a)
        {
            Debug.Log(a);
        }


        [AnimationEvent("get position", EAnimationEventParameter.Vector3)]
        public void GetPosition(Vector3 a)
        {
            Debug.Log(a);
        }

        
        [AnimationEvent("get layerMask", EAnimationEventParameter.LayerMask)]
        public void GetLayerMask(int a)
        {
            Debug.Log(a);
        }
        
        
        [AnimationEvent("get tag", EAnimationEventParameter.Tag)]
        public void GetTag(int a)
        {
            Debug.Log(a);
        }

        
        [AnimationEvent("get Animation Curve", EAnimationEventParameter.AnimationCurve)]
        public void GetAnimationCurve(AnimationCurve a)
        {
            
        }
        
        
        [AnimationEvent("get SO", EAnimationEventParameter.ScriptableObject)]
        public void GetAnimationCurve(ScriptableObject a)
        {
            
        }
        
        [AnimationEvent("get Custom1", EAnimationEventParameter.Customization, typeof(CustomAnimEventParamExample1))]
        public void GetCustom(CustomAnimEventParamExample1 a)
        {
            
        }
        
        
        [AnimationEvent("get Custom2", EAnimationEventParameter.Customization, typeof(CustomAnimEventParamExample2))]
        public void GetCustom2(CustomAnimEventParamExample2 a)
        {
            
        }
    }
}