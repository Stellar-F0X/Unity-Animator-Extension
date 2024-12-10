using AnimatorExpansion;
using AnimatorExpansion.Parameters;
using UnityEngine;

public class EventLogger : MonoBehaviour
{
    public string log0Message;
    public string log1Message;
    public string log2Message;
    
    
    [AnimationEvent("Log0", EParameterType.Void)]
    public void Print()
    {
        Debug.Log(log0Message);
    }


    [AnimationEvent("Log1", EParameterType.Void)]
    public void Log1()
    {
        Debug.Log(log1Message);
    }

    
    [AnimationEvent("Log2", EParameterType.Void)]
    public void Log2()
    {
        Debug.Log(log2Message);
    }
}
