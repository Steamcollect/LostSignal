using UnityEngine;

public class EntityCombat : MonoBehaviour, ILookAtTarget
{
    [Header("Settings")]
    [SerializeField] float turnSmoothTime;
    [SerializeField] float angleOffset;
    float turnSmoothVelocity;

    [Header("References")]
    [SerializeField] Transform rotationPivot;

    public void LookAt(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(rotationPivot.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        rotationPivot.rotation = Quaternion.Euler(0, angle, 0);
    }
}
