using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RangeOverload_CombatStyle : CombatStyle
{
    [Header("RANGE OVERLOAD")]
    [SerializeField, MVsToolkit.Dev.ReadOnly] float m_CurentTemperature;
    [SerializeField, MVsToolkit.Dev.ReadOnly] WeaponState m_CurrentState;
    enum WeaponState
    {
        CanShoot,
        Overload,
        DefaultCool,
        OverloadCool,
        CoolBuffed,
        CoolNerfed
    }

    [Space(10)]
    [SerializeField, Tooltip("Delay between attacks")] float m_AttackCooldown;

    [Space(5)]
    [SerializeField, Tooltip("Temperature donnée par shoot")] float m_ShootTemperature;
    
    [Space(10)]
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_DefaultCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_OverloadCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand input raté")] float m_NerfCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand input réussis")] float m_BuffCoolsPerSec;

    [Space(5)]
    [SerializeField] Vector2 m_RangeToReset;
    [SerializeField] Vector2 m_RangeToBuff;
    [SerializeField] Vector2 m_RangeToNerf;

    [Space(10)]
    [FoldoutGroup("Visual"), SerializeField] MeshRenderer m_MeshRenderer;
    [FoldoutGroup("Visual"), SerializeField] Gradient m_ColorOverTemperature;

    [FoldoutGroup("References"), SerializeField] Transform m_AttackPoint;

    [FoldoutGroup("References"), SerializeField] GameObject m_MuzzleFlashPrefab;
    [FoldoutGroup("References"), SerializeField] Bullet m_BulletPrefab;

    [Space(10)]
    [FoldoutGroup("References"), SerializeField] UnityEvent m_OnAttackFeedback;
    [FoldoutGroup("References"), SerializeField] UnityEvent m_OnReloadFeedback;

    Material m_RendererMat;

    [Header("Input")]
    [SerializeField] InputActionReference m_HandleCoolsInput;
    [SerializeField] InputActionReference m_HandleCoolsSkillInput;

    //[Header("Output")]

    private void OnEnable()
    {
        m_HandleCoolsInput.action.started += Cool;
        m_HandleCoolsSkillInput.action.started += TryBuffOnCool;
    }

    private void OnDisable()
    {
        m_HandleCoolsInput.action.started -= Cool;
        m_HandleCoolsSkillInput.action.started -= TryBuffOnCool;
    }

    private void Start()
    {
        m_RendererMat = new Material(m_MeshRenderer.material);
        m_MeshRenderer.material = m_RendererMat;
        SetRendererColor();
    }

    private void Update()
    {
        switch (m_CurrentState)
        {
            case WeaponState.DefaultCool:
                HandleCool(m_DefaultCoolsPerSec);
                break;
            case WeaponState.OverloadCool:
                HandleCool(m_DefaultCoolsPerSec);
                break;

            case WeaponState.CoolBuffed:
                HandleCool(m_BuffCoolsPerSec);
                break;
            case WeaponState.CoolNerfed:
                HandleCool(m_NerfCoolsPerSec);
                break;
        }
    }

    void HandleCool(float coolSpeed)
    {
        m_CurentTemperature -= coolSpeed * Time.deltaTime;

        if(m_CurentTemperature <= 0)
        {
            m_CurentTemperature = 0;
            m_CurrentState = WeaponState.CanShoot;
        }
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack 
            && (m_CurrentState == WeaponState.CanShoot || m_CurrentState == WeaponState.CoolBuffed))
        {
            OnAttack?.Invoke();
            
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            StartCoroutine(AttackCooldown());
            
            m_OnAttackFeedback?.Invoke();

            m_CurentTemperature += m_ShootTemperature;
            if (m_CurentTemperature >= 100)
                OnOverload();

            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);

            yield break;
        }
    }

    void OnOverload()
    {
        m_CurentTemperature = 100;
        m_CurrentState = WeaponState.Overload;
    }

    void Cool(InputAction.CallbackContext ctx)
    {
        switch (m_CurrentState)
        {
            case WeaponState.CanShoot:
                m_CurrentState = WeaponState.DefaultCool;
                break;
            case WeaponState.Overload:
                m_CurrentState = WeaponState.OverloadCool;
                break;
        }
    }

    void TryBuffOnCool(InputAction.CallbackContext ctx)
    {
        if (m_CurrentState != WeaponState.OverloadCool) return;

        if (m_CurentTemperature.InRange(m_RangeToBuff))
        {
            m_CurrentState = WeaponState.CoolBuffed;
            m_CurentTemperature = 100;
        }
        else if (m_CurentTemperature.InRange(m_RangeToReset))
        {
            m_CurrentState = WeaponState.CanShoot;
            m_CurentTemperature = 0;
        }
        else if (m_CurentTemperature.InRange(m_RangeToNerf))
        {
            m_CurrentState = WeaponState.CoolNerfed;
            m_CurentTemperature = 100;
        }
    }

    private IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    private void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature * .001f);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }

    private void OnValidate()
    {
        m_RangeToReset.x = Mathf.Clamp(m_RangeToReset.x, 0, m_RangeToReset.y);
        m_RangeToReset.y = Mathf.Clamp(m_RangeToReset.y, m_RangeToReset.x, 100);

        m_RangeToBuff.x = Mathf.Clamp(m_RangeToBuff.x, 0, m_RangeToBuff.y);
        m_RangeToBuff.y = Mathf.Clamp(m_RangeToBuff.y, m_RangeToBuff.x, 100);

        m_RangeToNerf.x = Mathf.Clamp(m_RangeToNerf.x, 0, m_RangeToNerf.y);
        m_RangeToNerf.y = Mathf.Clamp(m_RangeToNerf.y, m_RangeToNerf.x, 100);
    }
}