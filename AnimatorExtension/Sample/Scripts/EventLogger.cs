using System;
using System.Collections.Generic;
using AnimatorExtension.Parameters;
using UnityEngine;
using AnimationInfo = AnimatorExtension.Parameters.AnimationInfo;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AnimatorExtension.Sample
{
    public class EventLogger : MonoBehaviour
    {
        private Animator animator;

        public string log0Message;
        

        [AnimationEvent("Log")]
        public void Print()
        {
            Debug.Log(log0Message);
        }


        [AnimationEvent("Print Message", EAnimationEventParameter.String)]
        public void LogIntMessage(string a)
        {
            Debug.Log(a);
        }


        [AnimationEvent("Get Quaternion", EAnimationEventParameter.Quaternion)]
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
        public void GetLayerMask(LayerMask a)
        {
            Debug.Log($"LayerMask : {a.value}");
        }
        
        
        [AnimationEvent("get tag", EAnimationEventParameter.Tag)]
        public void GetTag(string a)
        {
            Debug.Log($"Tag : {a}");
        }

        
        [AnimationEvent("get Animation Curve", EAnimationEventParameter.AnimationCurve)]
        public void GetAnimationCurve(AnimationCurve a)
        {
            Debug.Log(a.keys.Length);
        }
        
        
        [AnimationEvent("get Animator Info", EAnimationEventParameter.AnimatorInfo)]
        public void GetAnimatorInformation(AnimationInfo a)
        {
            Debug.Log($"Name : {a.nodeName}    Tag : {a.nodeTag}    LayerIndex : {a.layerIndex} IsTransitioning : {a.isTransitioning}");
        }
        
        
        [AnimationEvent("get Object", EAnimationEventParameter.Object)]
        public void GetObject(Object a)
        {
            Debug.Log(a.name);
        }
        
        
        [AnimationEvent("get Custom1", EAnimationEventParameter.Customization, typeof(CustomAnimEventParamExample1))]
        public void GetCustom(CustomAnimationEventParameter param)
        {
            var p = (CustomAnimEventParamExample1)param; 
            Debug.Log($"{p.a} {p.b} {p.c} {p.d}");
        }
        
        
        [AnimationEvent("get Custom2", EAnimationEventParameter.Customization, typeof(CustomAnimEventParamExample2))]
        public void GetCustom2(CustomAnimationEventParameter param)
        {
            var p = (param as CustomAnimEventParamExample2);
            Debug.Log($"{p.eventName} {p.eventParameter} {p.nameList.Count}");
        }
    }
}