using System;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension.Sample
{
    [Serializable]
    public class CustomAnimEventParamExample1 : CustomAnimationEventParameter
    {
        public string a;
        public int b;

        [Range(0f, 1f)]
        public float c;

        public bool d;


        public override void OnBeforeEventTrigger()
        {
            Debug.Log($"{a} {b} {c} {d}");
        }
    }
}