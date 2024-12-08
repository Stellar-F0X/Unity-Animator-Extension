using AnimatorExpansion;
using AnimatorExpansion.Parameters;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public string logMessage;
    
    [AnimationEvent("Print", EParameterType.Void)]
    public void Print()
    {
        Debug.Log(logMessage);
    }
}
