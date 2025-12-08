using System;
using UnityEngine;

public class Laser : CombatStyle
{
    [Header("Laser Settings")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private LineRenderer m_LaserBeamLine;
    [SerializeField] private float m_LaserRange = 50f;
    [SerializeField] private int m_DamagePerSecond = 10;
    [SerializeField] private float m_Knockback = 0f;

    private void Start()
    {
        m_LaserBeamLine.enabled = false;
    }

    public override void Attack()
    {
        if (!canAttack) return;

        if (CurrentMana.Get() >= manaCostPerAttack)
        {
            CurrentMana.Set(CurrentMana.Get() - (manaCostPerAttack * Time.deltaTime));
        }
        else
        {
            return;
        }
        
        isAttacking = true;
        m_LaserBeamLine.enabled = true;

        RaycastHit hit;
        Vector3 endPosition = m_AttackPoint.position + m_AttackPoint.forward * m_LaserRange;

        if (Physics.Raycast(m_AttackPoint.position, m_AttackPoint.forward, out hit, m_LaserRange))
        {
            endPosition = hit.point;

            if (hit.collider.TryGetComponent(out EntityTrigger trigger))
            {
                if (m_Knockback > 0)
                {
                    trigger.GetController().GetRigidbody().AddForce(transform.up * m_Knockback);
                }
                trigger.GetController()?.GetHealth().TakeDamage(m_DamagePerSecond * Time.deltaTime);
            }
            
            if (hit.collider.TryGetComponent(out EnergySource energySource))
            {
                energySource.TakeDamage(m_DamagePerSecond * Time.deltaTime);
            }
        }

        // Update the laser beam line positions
        m_LaserBeamLine.SetPosition(0, m_AttackPoint.position);
        m_LaserBeamLine.SetPosition(1, endPosition);
    }
    
    public override void StopAttack()
    {
        isAttacking = false;
        m_LaserBeamLine.enabled = false;
    }
}
