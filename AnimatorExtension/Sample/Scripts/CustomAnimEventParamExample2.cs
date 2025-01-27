using System.Collections.Generic;
using AnimatorExtension.Parameters;
using UnityEngine;

namespace AnimatorExtension.Sample
{
    public class CustomAnimEventParamExample2 : CustomAnimationEventParameter
    {
        public string eventName;
        public int eventParameter;

        public List<string> nameList;

        public override void OnBeforeEventTrigger()
        {
            
        }
    }
}