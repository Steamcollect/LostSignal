using UnityEngine;
using System.Collections;

public class RangeOverloadCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] float maxCost;
    [SerializeField] float shootCost;
    float curentTemperature;

    [Space(10)]
    [SerializeField] int bulletDamage;
    [SerializeField] float bulletSpeed;
    [SerializeField] float knockBackForce;

    [Space(10)]
    [SerializeField] float attackCooldown;
    [SerializeField] float overloadCooldown;
    [SerializeField] float timeToCoolsAfterShoot;

    bool canAttack = true;
    bool isReloading = false;

    float coolsTimer;

    [Header("References")]
    [SerializeField] Transform attackPoint;

    [SerializeField] private RangeReloadingWeaponSFXManager m_SFXManager;

    //[Header("Input")]
    //[Header("Output")]

    private void Update()
    {
        coolsTimer += Time.deltaTime;

        if(coolsTimer > timeToCoolsAfterShoot)
        {

        }
    }

    public override void Attack()
    {
        if (canAttack && !isReloading)
        {
            coolsTimer = 0;
            OnAMMOChange?.Invoke(shootCost, maxCost);

            Bullet bullet = BulletManager.Instance.GetBullet();
            bullet.transform.position = attackPoint.position;
            bullet.transform.up = attackPoint.forward;

            bullet.Setup(bulletDamage, bulletSpeed)
                .SetKnockback(knockBackForce);

            StartCoroutine(AttackCooldown());

            if (m_SFXManager)
                m_SFXManager.PlayAttackSFX();
        }
    }

    public void Cools()
    {
        if (!isReloading)
        {
            OnReload?.Invoke();

            if (m_SFXManager)
                m_SFXManager.PlayReloadSFX();
            StartCoroutine(ReloadCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator ReloadCooldown()
    {
        isReloading = true;
        yield return new WaitForSeconds(overloadCooldown);
        currentBulletCount = maxBulletCount;
        OnAMMOChange?.Invoke(currentBulletCount, maxBulletCount);
        isReloading = false;
    }
}