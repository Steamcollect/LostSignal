using System;
using System.Collections;
using UnityEngine;

public class BulletHellTurret : MonoBehaviour
{
    public GameObject muzzle;
    [SerializeField] private int bulletsPerShot = 5;
    [SerializeField] private float timeBetweenBullets = 0.1f;
    [SerializeField] private bool resetRotationAfterPattern = true;
    [SerializeField] private float timeBetweenPatterns = 5f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float bulletSpeed = 30f;
    
    private int currentShotIndex = 0;
    private float shootTimer = 0f; 
    
    private void Start()
    {
        StartCoroutine(RotationCoroutine());
        StartCoroutine(ShootRoutine());
    }

    private void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
    
    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            if (currentShotIndex >= bulletsPerShot)
            {
                yield return new WaitForSeconds(timeBetweenPatterns);
                currentShotIndex = 0;
                if (resetRotationAfterPattern)
                    ResetRotation();
            }
            else
            {
                yield return new WaitForSeconds(timeBetweenBullets);
            }
        }
    }
    
    private IEnumerator RotationCoroutine()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void Shoot()
    {
        var bullet = BulletManager.Instance.GetBullet();
        bullet.transform.position = muzzle.transform.position;
        bullet.transform.rotation = muzzle.transform.rotation;
        bullet.Setup(1, bulletSpeed);
        currentShotIndex++;
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
