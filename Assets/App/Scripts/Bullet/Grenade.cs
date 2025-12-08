using System;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float explosionRadius;
    [SerializeField] GameObject radiusVisualizer;
    private GameObject explosionEffect;
    
    public void ShowExplosionRadius(Vector3 position)
    {
        if (radiusVisualizer != null)
        {
            explosionEffect = Instantiate(radiusVisualizer, position, Quaternion.identity);
            explosionEffect.transform.localScale = Vector3.one * explosionRadius * 2f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Explode();
    }
    
    private void Explode()
    {
        Destroy(explosionEffect);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.gameObject.TryGetComponent(out EntityTrigger trigger) && !nearbyObject.gameObject.CompareTag("Player"))
            {
                trigger.GetController()?.GetHealth().TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
