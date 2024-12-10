using AnimatorExpansion;
using AnimatorExpansion.Parameters;
using UnityEngine;

public class EventLogger : MonoBehaviour
{
    public string log0Message;
    public string log1Message;
    public string log2Message;
    
    
    [AnimationEvent("Log", EParameterType.Void)]
    public void Print()
    {
        Debug.Log(log0Message);
    }

    [AnimationEvent("LogMessage", EParameterType.Int)]
    public void LogIntMessage(int a)
    {
        Debug.Log(a);
    }

    
    [AnimationEvent("GetQuaternion", EParameterType.Quaternion)]
    public void GetQuaternion(Quaternion a)
    {
        Debug.Log(a);
    }


    [AnimationEvent("get position", EParameterType.Vector3)]
    public void GetPosition(Vector3 a)
    {
        Debug.Log(a);
    }
}
