using System;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension.Sample
{
    [Serializable]
    public class ExampleCustomAnimationEventParameter : CustomAnimationEventParameter
    {
        public string a;
        public int b;

        [Range(0f, 1f)]
        public float c;

        public bool d;


        public override void Execute()
        {
            Debug.Log($"{a} {b} {c} {d}");
        }
    }
    
    [Serializable]
    public class ExampleCustomAnimationEventParameter2 : CustomAnimationEventParameter
    {
        public string a;
        public int b;


        public override void Execute()
        {
            Debug.Log($"{a} {b}");
        }
    }
}