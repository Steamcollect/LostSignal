using System.Collections;
using UnityEngine;

public class Entity_Dash : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float dashCooldown;

    [Space(10)]
    [SerializeField] ForceMode dashForceMode;

    [Space(10)]
    [SerializeField] float dashDrag;
    [SerializeField] float dashForce;
    [SerializeField] float dadhTime;
    [SerializeField] float invicibilityTime;

    float beginDrag;
    bool canDash = true;

    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] EntityHealth entityHealth;

    //[Header("Input")]
    //[Header("Output")]

    public void Dash(Vector3 input)
    {
        if (!canDash) return;

        beginDrag = rb.linearDamping;
        rb.linearDamping = dashDrag;
        
        entityHealth.invincibilityDelay = invicibilityTime;

        rb.AddForce(input * dashForce, dashForceMode);

        StartCoroutine(DashTime());
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashTime()
    {
        yield return new WaitForSeconds(dadhTime);
        entityHealth.invincibilityDelay = 0;
        rb.linearVelocity = rb.linearVelocity.normalized;
        rb.linearDamping = beginDrag;
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}