using MVsToolkit.Utils;
using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("Settings")]
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    public float invincibilityDelay;
    bool isInvincible = false;

    [Header("References")]
    [SerializeField] protected DamageSFXManager m_DamageSFXManager;
    
    public Action OnTakeDamage, OnDeath;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        //à faire plus proprement
        if (invincibilityDelay > 0)
        {
            Debug.Log("Entity is now invincible for " + invincibilityDelay + " seconds.");
            isInvincible = true;
            CoroutineUtils.Delay(this, () =>
            {
                isInvincible = false;
            }, invincibilityDelay);
            //Mettre feedbacks d'invincibilité
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
        OnDeath?.Invoke();
    }

    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;
    
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}