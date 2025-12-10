using System;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private List<EntityHealth> linkedObjects;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    public Material lineMaterial;
    void Start()
    {
        currentHealth = maxHealth;
        foreach (EntityHealth linkedObject in linkedObjects)
        {
            linkedObject.GainInvincibility();
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lineRenderers.Add(lr);
            lr.positionCount = 2;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f; 
            lr.material = lineMaterial;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, linkedObject.transform.position);
        }
    }

    private void Update()
    {
        for (int i = 0; i < linkedObjects.Count; i++)
        {
            if (linkedObjects[i] != null)
            {
                lineRenderers[i].SetPosition(0, transform.position);
                lineRenderers[i].SetPosition(1, linkedObjects[i].transform.position);
            }
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
