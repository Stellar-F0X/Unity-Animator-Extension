using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AnimatorExtension.Parameters
{
    [Serializable]
    public struct AnimationEventParameter
    {
        public EAnimationEventParameter parameterType;
        
        public int intValue;
        public bool boolValue;
        public string stringValue;
        public float floatValue;
        public Color colorValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Quaternion quaternionValue;
        public GameObject gameobjectValue;
        public AnimationCurve curveValue;
        public Object objectValue;
        public AnimationInfo animationInfoValue;
        
        [SerializeReference]
        public CustomAnimationEventParameter customValue;
    }
}