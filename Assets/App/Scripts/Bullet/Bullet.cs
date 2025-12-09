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

    //[Header("Input")]
    //[Header("Output")]

    private Vector3 m_OriginalPosition;
    
    public Vector3 GetShootPosition() => m_OriginalPosition;
    
    public Bullet Setup(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        m_OriginalPosition = transform.position;

        StartCoroutine(CheckDistanceFromPlayer());

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

        transform.position = Vector3.zero;
        BulletManager.Instance.ReturnBullet(this);
    }

    IEnumerator CheckDistanceFromPlayer()
    {
        yield return new WaitForSeconds(5);
        transform.position = Vector3.zero;
        BulletManager.Instance.ReturnBullet(this);
    }
}