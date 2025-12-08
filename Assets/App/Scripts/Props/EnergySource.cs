using System;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private List<EntityHealth> linkedObjects;
    void Start()
    {
        currentHealth = maxHealth;
        foreach (EntityHealth linkedObject in linkedObjects)
        {
            linkedObject.GainInvincibility();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (other.gameObject.TryGetComponent(out Bullet bullet))
            {
                TakeDamage(bullet.Damage);
            }
            
            if (other.gameObject.TryGetComponent(out Grenade grenade))
            {
                TakeDamage(grenade.Damage);
            }
            
            
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            foreach (EntityHealth linkedObject in linkedObjects)
            {
                linkedObject.LoseInvincibility();
            }
            Destroy(gameObject);
        }
    }
}
