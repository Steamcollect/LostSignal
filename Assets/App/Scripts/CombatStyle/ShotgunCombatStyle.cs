using System.Collections;
using UnityEngine;

public class ShotgunCombatStyle : CombatStyle
{
    [SerializeField] private int m_ShotsPerMagazine = 2;
    [SerializeField] private int m_BulletsPerShot = 8;
    [SerializeField] private float m_SpreadAngle = 15f;
    [SerializeField] private float m_AttackCooldown = 1f;
    [SerializeField] private float m_ReloadCooldown = 1f;

    [Space(10)]
    [SerializeField] int m_BulletDamage;
    [SerializeField] float m_BulletSpeed;
    [SerializeField] float m_KnockBackForce;
    
    [SerializeField] Transform m_AttackPoint;
    
    [SerializeField] private Bullet m_BulletPrefab;
    
    private int m_ShotsRemaining;

    private void Start()
    {
        m_ShotsRemaining = m_ShotsPerMagazine;
    }

    public override IEnumerator Attack()
    {
        if (!m_CanAttack) yield break;
        
        m_IsAttacking = true;
        m_CanAttack = false;
        
        OnAttack?.Invoke();
        
        for (int i = 0; i < m_BulletsPerShot; i++)
        {
            // Compute a regularly spaced yaw angle across the spread cone (degrees)
            float yaw;
            if (m_BulletsPerShot == 1)
            {
                yaw = 0f;
            }
            else
            {
                float step = m_SpreadAngle / (m_BulletsPerShot - 1);
                yaw = -m_SpreadAngle / 2f + step * i;
            }

            // Apply yaw relative to the AttackPoint's local rotation so spread follows the barrel orientation
            Quaternion spreadRotation = m_AttackPoint.rotation * Quaternion.Euler(0f, yaw, 0f);

            // Spawn the bullet with the calculated rotation
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, spreadRotation);

            bullet.Setup();
        }
        
        m_ShotsRemaining--;

        if (m_ShotsRemaining <= 0)
        {
            Reload();
        }
        else
        {
            StartCoroutine(AttackCooldown());
        }
    }
    
    public override void Reload()
    {
        StartCoroutine(ReloadCooldown());
        OnReload?.Invoke();
    }
    
    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(m_ReloadCooldown);
        m_ShotsRemaining = m_ShotsPerMagazine;
        m_CanAttack = true;
        m_IsAttacking = false;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
        m_IsAttacking = false;
    }
}
