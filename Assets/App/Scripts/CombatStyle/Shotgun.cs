using System.Collections;
using UnityEngine;

public class Shotgun : CombatStyle
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
    
    public override void Attack()
    {
        if (!canAttack) return;
        
        isAttacking = true;
        canAttack = false;
        
        OnAttack?.Invoke();
        
        for (int i = 0; i < m_BulletsPerShot; i++)
        {
            float angle = Random.Range(-m_SpreadAngle / 2, m_SpreadAngle / 2);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * m_AttackPoint.forward;

            Bullet bullet = BulletManager.Instance.GetBullet();
            bullet.transform.position = m_AttackPoint.position;
            bullet.transform.up = direction;

            bullet.Setup(m_BulletDamage, m_BulletSpeed)
                  .SetKnockback(m_KnockBackForce);
        }
        
        m_ShotsPerMagazine--;

        if (m_ShotsPerMagazine <= 0)
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
        canAttack = true;
        isAttacking = false;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(m_AttackCooldown);
        canAttack = true;
        isAttacking = false;
    }
}
