using UnityEngine;

namespace AnimatorExpansion.Parameters
{
    public struct SEventParameter
    {
        public EParameterType parameterType;
        public bool hasParameter;
        
        public int intValue;
        public char charValue;
        public bool boolValue;
        public string stringValue;
        public float floatValue;
        public Color colorValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Quaternion quaternionValue;
        public GameObject gameObjectValue;
        public Transform transformValue;
    }
}