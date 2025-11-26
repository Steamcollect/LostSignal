using UnityEngine;

public class RadioAntennaController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RadioAntennaState state;
    enum RadioAntennaState { Enable, Disable, Active}

    [Header("References")]
    [SerializeField] RadioAntennaTrigger trigger;

    private void OnEnable()
    {
        trigger.OnPlayerInteract += OnPlayerInteract;
    }
    private void OnDisable()
    {
        trigger.OnPlayerInteract -= OnPlayerInteract;
    }

    private void Start()
    {
        trigger.SetCanPlayerInteract(true);
    }

    void OnPlayerInteract()
    {
        Debug.Log("Player interact with " + gameObject.name);
    }
}