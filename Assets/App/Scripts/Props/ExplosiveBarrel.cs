using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private GameObject explosionEffect;

    private void Start()
    {
        explosionEffect.SetActive(false);
    }
    
    public LayerMask mask;
    public void Explode()
    {
        List<IHealth> entities = new List<IHealth>();

        StartCoroutine(ExplosionVFX());
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, mask);
        
        foreach (Collider collider in colliders)
        {
            IHealth trigger = collider.GetComponent<IHealth>();
            if (trigger != null && !entities.Contains(trigger))
            {
                entities.Add(trigger);
            }
        }
        
        foreach (IHealth entity in entities)
        {
            Debug.Log("Damaging entity: " + entity.name);
            entity.GetController().GetHealth().TakeDamage(explosionDamage);
        }
    }

    public IEnumerator ExplosionVFX()
    {
        explosionEffect.transform.localScale = Vector3.zero;
        explosionEffect.SetActive(true);
        float elapsedTime = 0f;
        float duration = 0.5f;
        while (elapsedTime < duration)
        {
            explosionEffect.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * explosionRadius * 2f, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        explosionEffect.transform.localScale = Vector3.one * explosionRadius * 2f;
        
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
