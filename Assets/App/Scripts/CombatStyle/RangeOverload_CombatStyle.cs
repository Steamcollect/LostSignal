using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RangeOverload_CombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_DefaultCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_OverloadCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand input raté")] float m_NerfCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand int réussis")] float m_BuffCoolsPerSec;

    [Space(10)]
    [SerializeField] float m_ShootTemperature;
    [SerializeField] float m_TemperatureLostPerSec;

    [Space(5)] 
    [SerializeField] float m_AttackCooldown;

    [SerializeField] float m_OverloadCooldown;
    [SerializeField] float m_OverloadRecorverySpeed;
    [SerializeField] float m_TimeToCoolsAfterShoot;

    [Space(30), Header("========================="), Space(30), Header("Visual")]
    [SerializeField] MeshRenderer m_MeshRenderer;
    [SerializeField] Gradient m_ColorOverTemperature;

    [Header("References")]
    [SerializeField] Transform m_AttackPoint;

    [SerializeField] GameObject m_MuzzleFlashPrefab;
    [SerializeField] Bullet m_BulletPrefab;

    [Space(10)]
    [SerializeField] UnityEvent m_OnAttackFeedback;
    [SerializeField] UnityEvent m_OnReloadFeedback;

    float m_CoolsTimer;
    float m_CurentTemperature;

    bool m_IsOverload;

    Material m_RendererMat;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_RendererMat = new Material(m_MeshRenderer.material);
        m_MeshRenderer.material = m_RendererMat;
        SetRendererColor();
    }

    private void Update()
    {
        m_CoolsTimer += Time.deltaTime;

        if (m_CoolsTimer > m_TimeToCoolsAfterShoot && !m_IsOverload)
        {
            m_CurentTemperature =
                Mathf.Clamp(m_CurentTemperature - m_TemperatureLostPerSec * Time.deltaTime, 0, 100);
            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);
        }
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack && !m_IsOverload)
        {
            OnAttack?.Invoke();
            m_CoolsTimer = 0;
            
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            StartCoroutine(AttackCooldown());
            
            m_OnAttackFeedback?.Invoke();

            m_CurentTemperature += m_ShootTemperature;
            if (m_CurentTemperature >= 100)
            {
                m_CurentTemperature = 100;
                Overload();
            }

            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);

            yield break;
        }
    }

    public void Overload()
    {
        if (!m_IsOverload)
        {
            OnReload?.Invoke();

            m_OnReloadFeedback?.Invoke();
            StartCoroutine(OverloadCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    private IEnumerator OverloadCooldown()
    {
        m_IsOverload = true;
        yield return new WaitForSeconds(m_OverloadCooldown);

        while (m_CurentTemperature > 0)
        {
            m_CurentTemperature -= m_OverloadRecorverySpeed * Time.deltaTime;
            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);
            yield return null;
        }

        m_CurentTemperature = 0;

        m_IsOverload = false;
    }

    private void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature * .001f);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }
}