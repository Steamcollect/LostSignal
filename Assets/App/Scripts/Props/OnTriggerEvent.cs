using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent enterEvent;

    public UnityEvent exitEvent;

    private void OnTriggerEnter(Collider other)
    {
        enterEvent.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        exitEvent.Invoke();
    }
}
