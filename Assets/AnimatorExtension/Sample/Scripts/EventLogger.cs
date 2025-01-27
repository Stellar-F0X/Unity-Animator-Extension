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
        

        [AnimationEvent("Log")]
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
        
        
        [AnimationEvent("get SO", EAnimationEventParameter.ScriptableObject)]
        public void GetScriptableObject(ScriptableObject a)
        {
            Debug.Log(a.name);
        }
        
        
        [AnimationEvent("get Custom1", EAnimationEventParameter.Customization, typeof(CustomAnimEventParamExample1))]
        public void GetCustom(CustomAnimationEventParameter param)
        {
            var p = (param as CustomAnimEventParamExample1); 
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