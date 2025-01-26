using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimatorExtension.Parameters
{
    [Serializable]
    public struct AnimationEventParameter
    {
        public EAnimationEventParameter parameterType;
        
        public int intValue;
        public char charValue;
        public bool boolValue;
        public string stringValue;
        public float floatValue;
        public Color colorValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Quaternion quaternionValue;
        public GameObject gobjValue;
        public Transform transformValue;
        public AnimationCurve curveValue;
        public ScriptableObject sobjValue;
        
        [SerializeReference]
        public CustomAnimationEventParameter customValue;
    }
}