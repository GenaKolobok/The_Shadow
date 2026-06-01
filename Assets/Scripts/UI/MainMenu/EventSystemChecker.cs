using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
    private void Start()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem not found! UI buttons will not work. Please add an EventSystem to the scene.");

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            Debug.Log("EventSystem has been automatically created.");
        }
        else
        {
            Debug.Log("EventSystem found and working correctly.");
        }
    }
}
