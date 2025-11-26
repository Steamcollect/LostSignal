using UnityEngine;

public class EntityTrigger : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    EntityController controller;

    //[Header("Input")]
    //[Header("Output")]

    public void SetController(EntityController controller)
    {
        this.controller = controller;
    }

    public EntityController GetController() {  return controller; }
}