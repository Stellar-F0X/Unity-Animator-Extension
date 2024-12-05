using UnityEngine;

public class Logger : MonoBehaviour
{
    public string logMessage;
    
    public void Print()
    {
        Debug.Log(logMessage);
    }
}
