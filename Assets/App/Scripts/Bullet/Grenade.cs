using System;
using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] int m_Damage;
    [SerializeField] float explosionRadius;
    [SerializeField] GameObject radiusVisualizer;
    
    private GameObject explosionEffect;
    private Vector3 m_Destination;
    
    public int Damage
    {
        get => m_Damage;
        set => m_Damage = value;
    }

    public void ShowExplosionRadius(Vector3 position)
    {
        if (radiusVisualizer != null)
        {
            explosionEffect = Instantiate(radiusVisualizer, position, Quaternion.identity);
            explosionEffect.transform.localScale = Vector3.one * explosionRadius * 2f;
        }
    }

    public void SetDestination(Vector3 position)
    {
        m_Destination = position;
    }

    public void Fire()
    {
        ShowExplosionRadius(m_Destination);
        StartCoroutine(MissileRoutine());
    }

    [SerializeField] private AnimationCurve m_ApexTrajectory, m_SplitTrajectory, m_TargetTrajectory;
    private float m_HeightTarget = 10;
    private float m_HorizontalDistanceBeforeSplit = 2f;
    
    private IEnumerator MissileRoutine()
    {
        Vector3 apexPoint = transform.position;
        apexPoint.y += m_HeightTarget;
        
        Vector3 splitPoint = m_Destination;
        splitPoint.y += m_HeightTarget;
        
        Vector3 direction = (splitPoint - apexPoint).normalized;
        Vector3 offsetBeforeSplit = direction * m_HorizontalDistanceBeforeSplit;
        
        splitPoint -= offsetBeforeSplit;
        
        Vector3 lastPosition = transform.position;
        Vector3 moveDirection;
        Vector3 origin = transform.position;
        float timer = 0f;
        while (timer < m_ApexTrajectory.keys[m_ApexTrajectory.length - 1].time)
        {
            float heightFactor = m_ApexTrajectory.Evaluate(timer);
            Vector3 targetPosition = Vector3.Lerp(origin, apexPoint, heightFactor);
            transform.position = targetPosition;
            moveDirection = transform.position - lastPosition;
            transform.rotation = Quaternion.LookRotation(moveDirection.normalized, transform.up);
            lastPosition = transform.position;
            timer += Time.deltaTime;
            yield return null;
        }
        
        origin = transform.position;
        timer = 0f;
        while (timer < m_SplitTrajectory.keys[m_SplitTrajectory.length - 1].time)
        {
            float heightFactor = m_SplitTrajectory.Evaluate(timer);
            Vector3 targetPosition = Vector3.Lerp(origin, splitPoint, heightFactor);
            transform.position = targetPosition;
            moveDirection = transform.position - lastPosition;
            transform.rotation = Quaternion.LookRotation(moveDirection.normalized, transform.up);
            lastPosition = transform.position;
            timer += Time.deltaTime;
            yield return null;
        }

        origin = transform.position;
        timer = 0f;
        while (timer < m_ApexTrajectory.keys[m_ApexTrajectory.length - 1].time)
        {
            float heightFactor = m_TargetTrajectory.Evaluate(timer);
            Vector3 targetPosition = Vector3.Lerp(origin, m_Destination, heightFactor);
            transform.position = targetPosition;
            moveDirection = transform.position - lastPosition;
            transform.rotation = Quaternion.LookRotation(moveDirection.normalized, transform.up);
            lastPosition = transform.position;
            timer += Time.deltaTime;
            yield return null;
        }
        
        Explode();
    }
    
    private void Explode()
    {
        Destroy(explosionEffect);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.gameObject.TryGetComponent(out HurtBox hurtBox) && !nearbyObject.CompareTag("Player"))
            {
                hurtBox.TakeDamage(m_Damage);
            }
        }
        Destroy(gameObject);
    }
}