using System;
using MVsToolkit.Dev;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealth;

    [Header("References")]
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