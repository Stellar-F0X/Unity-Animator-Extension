using System;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension.Sample
{
    public class CustomAnimEventParamExample1 : CustomAnimationEventParameter
    {
        public string a;
        public int b;

        [Range(0f, 1f)]
        public float c;
        public bool d;


        public override void OnBeforeEventTrigger()
        {
            Debug.Log($"[before event] {a} {b} {c} {d}");
        }

        public override void OnAfterEventTrigger()
        {
            Debug.Log($"[after event] {a} {b} {c} {d}");
        }
    }
}