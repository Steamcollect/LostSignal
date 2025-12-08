using System.Collections;
using UnityEngine;

public class RangeReloadingCombatStyle : CombatStyle
{
    [Header("Bullet")]
    [SerializeField] private int m_MaxBulletCount;
    [SerializeField] private GameObject m_BulletPrefab;
    private int m_CurrentBulletCount;

    [Space(10)]
    [SerializeField] private int m_BulletDamage;
    [SerializeField] private float m_BulletSpeed;
    [SerializeField] private float m_KnockBackForce;

    [Space(10)]
    [SerializeField] private float m_AttackCooldown;
    [SerializeField] private float m_ReloadCooldown;

    private bool m_IsReloading = false;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private GameObject m_MuzzleFlashPrefab;

    [SerializeField] private RangeReloadingWeaponSFXManager m_SFXManager;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(.1f);
        m_CurrentBulletCount = m_MaxBulletCount;
    }

    public override void Attack()
    {
        if(m_CanAttack && !m_IsReloading)
        {
            if(m_CurrentBulletCount > 0)
            {
                OnAttack?.Invoke();

                m_CurrentBulletCount--;
                OnAmmoChange?.Invoke(m_CurrentBulletCount, m_MaxBulletCount);
                var muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
                Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);
                
                Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, Quaternion.identity).GetComponent<Bullet>();
                bullet.transform.up = m_AttackPoint.forward;

                bullet.Setup(m_BulletDamage, m_BulletSpeed)
                    .SetKnockback(m_KnockBackForce);

                StartCoroutine(AttackCooldown());
                
                if(m_SFXManager)
                    m_SFXManager.PlayAttackSfx();
            }
            else
            {
                Reload();
            }
        }
    }

    public override void Reload()
    {
        if (!m_IsReloading)
        {
            OnReload?.Invoke();

            if(m_SFXManager)
                m_SFXManager.PlayReloadSfx();
            StartCoroutine(ReloadCooldown());         
        }
    }

    IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    IEnumerator ReloadCooldown()
    {
        m_IsReloading = true;
        yield return new WaitForSeconds(m_ReloadCooldown);
        m_CurrentBulletCount = m_MaxBulletCount;
        OnAmmoChange?.Invoke(m_CurrentBulletCount, m_MaxBulletCount);
        m_IsReloading = false;
    }
}