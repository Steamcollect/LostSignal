using System.Collections;
using UnityEngine;

public class RangeReloadingCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] int maxBulletCount;
    int currentBulletCount;

    [Space(10)]
    [SerializeField] int bulletDamage;
    [SerializeField] float bulletSpeed;
    
    [Space(10)]
    [SerializeField] float attackCooldown;
    [SerializeField] float reloadCooldown;

    bool canAttack = true;
    bool isReloading = false;

    [Header("References")]
    [SerializeField] Transform attackPoint;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        currentBulletCount = maxBulletCount;
    }

    public override void Attack()
    {
        if(canAttack && !isReloading)
        {
            if(currentBulletCount > 0)
            {
                currentBulletCount--;

                Bullet bullet = BulletManager.Instance.GetBullet();
                bullet.transform.position = attackPoint.position;
                bullet.transform.up = attackPoint.forward;

                bullet.Setup(bulletDamage, bulletSpeed);

                StartCoroutine(AttackCooldown());
            }
            else
            {
                Reload();
            }
        }
    }

    public override void Reload()
    {
        if (!isReloading)
        {
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
        yield return new WaitForSeconds(reloadCooldown);
        currentBulletCount = maxBulletCount;
        isReloading = false;
    }
}