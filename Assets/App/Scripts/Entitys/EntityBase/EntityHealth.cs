using System;
using MVsToolkit.Dev;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealth;

    [Header("References")]
    [SerializeField] InterfaceReference<IShield> shield;
    [SerializeField] private DamageSFXManager m_DamageSFXManager;
    
    public Action OnTakeDamage, OnDeath;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (shield.Value != null && !shield.Value.IsDestroy())
        {
            int remainingDmg = shield.Value.TakeDamage(damage);
            if(remainingDmg > 0) TakeDamage(remainingDmg);
            
            return;
        }

        currentHealth -= damage;
        
        if(currentHealth <= 0)
        {
            Die();
        }
        else
        {
            m_DamageSFXManager.PlayDamageSFX();
            OnTakeDamage?.Invoke();
        }
    }

    void Die()
    {
        m_DamageSFXManager.PlayDeathSFX();
        OnDeath.Invoke();
    }
}