using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RangeOverloadCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] private float m_MaxTemperature;
    [SerializeField] private float m_ShootTemperature;
    [SerializeField] private float m_TemperatureLostPerSec;
    private float m_CurentTemperature;

    [Space(5)]
    [SerializeField] private float m_AttackCooldown;
    [SerializeField] private float m_OverloadCooldown;
    [SerializeField] private float m_OverloadRecorverySpeed;
    [SerializeField] private float m_TimeToCoolsAfterShoot;

    [Header("Bullet")]
    [SerializeField] private int m_BulletDamage;
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private float m_BulletSpeed;
    [SerializeField] private float m_KnockBackForce;

    private bool m_IsOverload = false;

    private float m_CoolsTimer;

    [Header("Visual")]
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private Gradient m_ColorOverTemperature;
    private Material m_RendererMat;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private GameObject m_MuzzleFlashPrefab;

    [SerializeField] private RangeReloadingWeaponSFXManager m_SFXManager;
    [SerializeField] private UnityEvent m_OnAttackFeedback;

    private void Start()
    {
        m_RendererMat = new Material(m_MeshRenderer.material);
        m_MeshRenderer.material = m_RendererMat;
        SetRendererColor();
    }

    private void Update()
    {
        m_CoolsTimer += Time.deltaTime;

        if(m_CoolsTimer > m_TimeToCoolsAfterShoot && !m_IsOverload)
        {
            m_CurentTemperature = Mathf.Clamp(m_CurentTemperature - m_TemperatureLostPerSec * Time.deltaTime, 0, m_MaxTemperature);
            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, m_MaxTemperature);
        }
    }

    public override void Attack()
    {
        if (m_CanAttack && !m_IsOverload)
        {
            m_OnAttackFeedback?.Invoke();
            OnAttack?.Invoke();
            m_CoolsTimer = 0;

            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.transform.up = m_AttackPoint.forward;
            GameObject muzzleVFX = PoolManager.Instance.Spawn(m_MuzzleFlashPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup(m_BulletDamage, m_BulletSpeed)
                .SetKnockback(m_KnockBackForce);

            StartCoroutine(AttackCooldown());

            if (m_SFXManager)
                m_SFXManager.PlayAttackSfx();

            m_CurentTemperature += m_ShootTemperature;
            if(m_CurentTemperature >= m_MaxTemperature)
            {
                m_CurentTemperature = m_MaxTemperature;
                Overload();
            }

            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, m_MaxTemperature);
        }
    }

    public void Overload()
    {
        if (!m_IsOverload)
        {
            OnReload?.Invoke();

            if (m_SFXManager)
                m_SFXManager.PlayReloadSfx();
            StartCoroutine(OverloadCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    IEnumerator OverloadCooldown()
    {
        m_IsOverload = true;
        yield return new WaitForSeconds(m_OverloadCooldown);

        while(m_CurentTemperature > 0)
        {
            m_CurentTemperature -= m_OverloadRecorverySpeed * Time.deltaTime;
            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, m_MaxTemperature);
            yield return null;
        }
        m_CurentTemperature = 0;

        m_IsOverload = false;
    }

    void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature / m_MaxTemperature);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }
}