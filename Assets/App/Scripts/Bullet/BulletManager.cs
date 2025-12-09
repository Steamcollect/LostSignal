using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int startingBulletCount = 30;

    [Header("References")]
    [SerializeField] Bullet bulletPrefab;
    Queue<Bullet> bullets = new();

    //[Header("Input")]
    //[Header("Output")]

    public static BulletManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < startingBulletCount; i++)
        {
            Bullet bullet = CreateBullet();
            bullet.gameObject.SetActive(false);
            bullets.Enqueue(bullet);
        }
    }

    public Bullet GetBullet()
    {
        if(bullets.Count <= 0)
        {
            return CreateBullet();
        }

        Bullet bullet = bullets.Dequeue();
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(Bullet bullet)
    {
        // Ensure bullet state is reset before reusing
        if (bullet != null)
        {
            bullet.ResetBullet();
            bullet.gameObject.SetActive(false);
            bullets.Enqueue(bullet);
        }
    }

    Bullet CreateBullet()
    {
        return Instantiate(bulletPrefab, transform);
    }
}