using AnimatorExtension;
using UnityEngine;
using UnityEngine.Profiling;

public class EventRegistryProfiler : MonoBehaviour
{
    public int objectCount;
    
    public GameObject createObjectPrefab;
    
    public AnimationEventController[] createObjects;


    public void Awake()
    {
        createObjects = new AnimationEventController[objectCount];
    }

    private void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            GameObject gobj = Instantiate(createObjectPrefab, this.transform);
            createObjects[i] = gobj.GetComponent<AnimationEventController>();
        }
    }

    public void Register()
    {
        Profiler.BeginSample("Create And Register Logic");
        
        for (int i = 0; i < objectCount; i++)
        {
            createObjects[i].RegisterEvents();
        }
        
        Profiler.EndSample();
    } 
}
