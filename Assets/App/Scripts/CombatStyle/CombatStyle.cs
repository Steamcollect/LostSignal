using System;
using UnityEngine;

public class CombatStyle : MonoBehaviour
{
    public Action<float /*current*/,float /*max*/> OnAmmoChange;
    public Action OnReload;

    public Action OnAttack;

    public float manaCostPerAttack = 0f;
    public RSO_Mana CurrentMana;
    protected bool canAttack = true;
    protected bool isAttacking = false;
    
    public virtual void Attack() { }
    public virtual void StopAttack() { }
    public virtual void Reload() { }

    public bool IsAttacking() => isAttacking;
    public bool CanAttack() => canAttack;
}