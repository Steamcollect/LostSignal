using System.Collections;
using DG.Tweening;
using MVsToolkit.Utils;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class CloseCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] int damage;
    [SerializeField] float attackCooldown;

    bool canAttack = true;

    [Header("References")]
    [SerializeField] Transform weaponPivot;
    [SerializeField] ColliderCallback callback;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        callback._OnTriggerEnter += OnWeaponTouchSomething;
    }

    public override void Attack()
    {
        if (canAttack)
        {
            StartCoroutine(AttackCooldown());
            weaponPivot.localRotation = Quaternion.identity;

            weaponPivot.gameObject.SetActive(true);
            weaponPivot.DOLocalRotate(
                new Vector3(0, -20, 0),
                .2f,
                RotateMode.FastBeyond360
            ).OnComplete(() =>
            {
                float rot = -20f;
                DOTween.To(() => rot, x => rot = x, 200f, 0.1f)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        weaponPivot.localRotation = Quaternion.Euler(0, rot, 0);
                    })
                    .OnComplete(() =>
                    {
                    CoroutineUtils.Delay(this, () =>
                    {
                        weaponPivot.gameObject.SetActive(false);
                    }, .2f);
                });
            });
        }
    }

    void OnWeaponTouchSomething(Collider collid)
    {
        if (collid.TryGetComponent(out EntityTrigger trigger))
        {
            trigger.GetController().GetHealth().TakeDamage(damage);
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}