using UnityEngine;
using MVsToolkit.Dev;

public class EntityController : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] protected EntityHealth health;
    [SerializeField] protected EntityTrigger trigger;
    [SerializeField] protected InterfaceReference<IMovement> movement;
    [SerializeField] protected EntityCombat combat;

    //[Header("Input")]
    //[Header("Output")]

    public EntityHealth GetHealth() {  return health; }
    public EntityTrigger GetTrigger() { return trigger; }
    public EntityCombat GetCombat() { return combat; }
    public IMovement GetMovement() { return movement.Value; }
}