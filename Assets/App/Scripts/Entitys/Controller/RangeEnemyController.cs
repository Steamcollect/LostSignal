using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.AI;

public class RangeEnemyController : EntityController
{
    [Header("Settings")]
    [SerializeField] float detectionRange;
    [SerializeField] LayerMask detectionMask;
    [SerializeField, TagName] string playertag;

    [Header("Internal References")]
    [SerializeField] NavMeshAgent agent;

    [Header("Input")]
    [SerializeField] RSO_PlayerController player;
    
    //[Header("Output")]

    private void FixedUpdate()
    {
        print(IsDetectingPlayer());
    }

    bool IsDetectingPlayer()
    {
        if (Vector3.Distance(player.Get().GetTargetPosition(), GetTargetPosition()) < detectionRange)
        {
            Vector3 dir = (player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
            Ray ray = new Ray(transform.position, dir);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, detectionRange, detectionMask))
            {
                if (hit.collider.CompareTag(playertag)) return true;
                else return false;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}