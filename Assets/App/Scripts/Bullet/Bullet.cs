using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    float speed;
    int damage;
    float knockback;
    
    public int Damage
    {
        get => damage;
        set => damage = value;
    }
    
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] private GameObject m_HitPrefab;

    // Track the distance coroutine so we can stop it when returning the bullet to the pool
    private Coroutine m_DistanceCoroutine;

    private Vector3 m_OriginalPosition;
    
    public Vector3 GetShootPosition() => m_OriginalPosition;
    
    public Bullet Setup(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        m_OriginalPosition = transform.position;

        // Ensure any previous distance-check coroutine is stopped before starting a new one
        if (m_DistanceCoroutine != null)
        {
            StopCoroutine(m_DistanceCoroutine);
            m_DistanceCoroutine = null;
        }

        m_DistanceCoroutine = StartCoroutine(CheckDistanceFromPlayer());

        return this;
    }
    public Bullet SetKnockback(float knockback)
    {
        this.knockback = knockback;
        return this;
    }

    private void Update()
    {
        rb.position += transform.up * (speed * Time.deltaTime);
    }
    
    public void Impact(GameObject target)
    {
        if (target.TryGetComponent(out IHealth health))
        {
            health.TakeDamage(damage);
        }
        
        // Make sure we stop any running coroutines / reset state before returning to pool
        ResetBullet();

        transform.position = Vector3.zero;
        BulletManager.Instance.ReturnBullet(this);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Bullet collided with " + other.gameObject.name);
        if (!other.gameObject.CompareTag("Bullet"))
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (m_HitPrefab && (!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player")))
            {
                GameObject hitVFX = Instantiate(m_HitPrefab, pos, rot);

                Destroy(hitVFX, hitVFX.GetComponent<ParticleSystem>().main.duration);
            }

            if (other.gameObject.TryGetComponent(out EntityTrigger trigger))
            {
                Debug.Log("Bullet hit entity with trigger: " + other.gameObject.name);
                if (knockback > 0)
                {
                    trigger.GetController().GetRigidbody().AddForce(transform.up * knockback);
                }
                trigger.GetController()?.GetHealth().TakeDamage(damage);
            }
            else if (other.gameObject.TryGetComponent(out EntityHealth health))
            {
                Debug.Log("Bullet hit entity with health: " + other.gameObject.name);
                health.TakeDamage(damage);
            }
        }

        // Ensure we stop coroutines / reset state before returning to pool
        ResetBullet();

        transform.position = Vector3.zero;
        BulletManager.Instance.ReturnBullet(this);
    }

    IEnumerator CheckDistanceFromPlayer()
    {
        yield return new WaitForSeconds(5);
        transform.position = Vector3.zero;
        BulletManager.Instance.ReturnBullet(this);

        // mark coroutine as finished
        m_DistanceCoroutine = null;
    }

    // Public helper to stop coroutines and reset internal state when returning to pool
    public void ResetBullet()
    {
        if (m_DistanceCoroutine != null)
        {
            try { StopCoroutine(m_DistanceCoroutine); } catch { }
            m_DistanceCoroutine = null;
        }

        // stop any other coroutines on this bullet to be safe
        StopAllCoroutines();

        // reset velocities
        if (rb != null)
        {
            rb.position = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // reset transform scale/rotation if needed (keeps pooling predictable)
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        // Make sure no coroutine is left running when disabled
        if (m_DistanceCoroutine != null)
        {
            try { StopCoroutine(m_DistanceCoroutine); } catch { }
            m_DistanceCoroutine = null;
        }
        StopAllCoroutines();
    }
}