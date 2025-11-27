using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.AI;

public class RollingEnemyController : EntityController
{
    [Header("Settings")]
    [SerializeField] float detectionRange;
    [SerializeField] float rotationTime;
    [SerializeField] int damage;

    Vector3 rollDir, dirVelocity;

    [SerializeField] float stunDelay;

    [Space(10)]
    [SerializeField] LayerMask detectionMask;
    [SerializeField, TagName] string playerTag;

    bool isChasingPlayer = false;

    bool isStun = false;

    [Header("References")]
    [SerializeField] RSO_PlayerController player;

    private void FixedUpdate()
    {
        if (!isStun && !isChasingPlayer)
        {
            if (IsPlayerInRange(detectionRange) && CanSeePlayer())
            {
                rollDir = (player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
                isChasingPlayer = true;
            }
            return;
        }

        if (isChasingPlayer)
        {
            Vector3 targetDir = (player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
            rollDir = Vector3.SmoothDamp(rollDir, targetDir, ref dirVelocity, rotationTime);

            combat.LookAt(transform.position + GetTargetPosition() + rollDir);
            movement.Value.Move(rollDir);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            if(collision.collider.TryGetComponent(out EntityTrigger trigger))
            {
                trigger.GetController().GetHealth().TakeDamage(damage);
            }
        }

        dirVelocity = Vector3.zero;
        StartCoroutine(StunCooldown());
        isChasingPlayer = false;
    }

    IEnumerator StunCooldown()
    {
        isStun = true;
        yield return new WaitForSeconds(stunDelay);
        isStun = false;
    }

    bool CanSeePlayer()
    {
        Vector3 dir = (player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
        Ray ray = new Ray(transform.position, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, detectionMask))
        {
            if (hit.collider.CompareTag(playerTag)) return true;
        }

        return false;
    }

    bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(player.Get().GetTargetPosition(), GetTargetPosition()) < range;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
