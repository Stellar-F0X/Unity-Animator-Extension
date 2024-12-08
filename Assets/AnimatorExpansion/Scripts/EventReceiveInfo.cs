using UnityEngine;
using UnityEngine.Events;
using AnimatorExpansion.Parameters;

namespace AnimatorExpansion
{
    public struct EventReceiveInfo
    {
        public string eventName;
        public int eventHash;
        public EParameterType parameterType;
        
        public UnityAction voidAction;
        public UnityAction<float> floatAction;
        public UnityAction<int> intAction;
        public UnityAction<bool> boolAction;
        public UnityAction<string> stringAction;
        public UnityAction<Vector2> vector2Action;
        public UnityAction<Vector3> vector3Action;
        public UnityAction<Quaternion> quaternionAction;
        public UnityAction<GameObject> gameObjectAction;
    }
}