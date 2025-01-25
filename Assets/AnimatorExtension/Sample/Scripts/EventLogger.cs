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

        

        [AnimationEvent("Log", EParameterType.Void)]
        public void Print()
        {
            Debug.Log(log0Message);
        }


        [AnimationEvent("LogMessage", EParameterType.Int)]
        public void LogIntMessage(int a)
        {
            Debug.Log(a);
        }


        [AnimationEvent("GetQuaternion", EParameterType.Quaternion)]
        public void GetQuaternion(Quaternion a)
        {
            Debug.Log(a);
        }


        [AnimationEvent("get position", EParameterType.Vector3)]
        public void GetPosition(Vector3 a)
        {
            Debug.Log(a);
        }

        
        [AnimationEvent("get layerMask", EParameterType.LayerMask)]
        public void GetLayerMask(int a)
        {
            Debug.Log(a);
        }
        
        
        [AnimationEvent("get tag", EParameterType.Tag)]
        public void GetTag(int a)
        {
            Debug.Log(a);
        }

        
        [AnimationEvent("get Animation Curve", EParameterType.AnimationCurve)]
        public void GetAnimationCurve(AnimationCurve a)
        {
            
        }
        
        
        [AnimationEvent("get SO", EParameterType.ScriptableObject)]
        public void GetAnimationCurve(ScriptableObject a)
        {
            
        }
        
        [AnimationEvent("get Custom", EParameterType.Customization, typeof(ExampleCustomParameter))]
        public void GetCustom(ExampleCustomParameter a)
        {
            
        }
    }
}