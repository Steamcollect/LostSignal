using System.Collections;
using UnityEngine;

/// <summary>
/// Simple GrenadeLauncher lisible et autonome.
/// Hérite toujours de CombatStyle (conserve intégration avec le système du projet).
/// Fonction : instancier la grenade au niveau du canon et lui donner une vitesse initiale
/// pour qu'elle suive une trajectoire en cloche et atterrisse sur la target.
/// </summary>
public class GrenadeLauncherCombatStyle : CombatStyle
{
    [Header("References")]
    [SerializeField] private GameObject m_GrenadePrefab;
    [SerializeField] private Transform m_FirePoint;
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;
    private Transform m_AimTargetTransform;
    
    [Tooltip("Temps de rechargement en secondes après le tir")]
    [SerializeField] private float m_ReloadTime = 2f;
    
    private void Start()
    {
        if (m_AimTarget != null)
            m_AimTargetTransform = m_AimTarget.Get();
    }

    // ------------------ API principale ------------------
    public override IEnumerator Attack()
    {
        if (!m_CanAttack) yield break;
        
        m_IsAttacking = true;
        
        if (!m_GrenadePrefab)
        {
            Debug.LogWarning("GrenadeLauncher: prefab de projectile non assigné.");
            yield break;
        }

        if (!m_FirePoint)
        {
            Debug.LogWarning("GrenadeLauncher: FirePoint non assigné.");
            yield break;
        }
        
        if (!m_AimTargetTransform)
        {
            Debug.LogWarning("GrenadeLauncher: AimTarget non assigné.");
            yield break;
        }
        
        Vector3 origin = m_FirePoint.position;
        Vector3 targetPos = m_AimTargetTransform.position;

        // Instancie la grenade
        Grenade grenade = Instantiate(m_GrenadePrefab, origin, m_FirePoint.rotation).GetComponent<Grenade>();
        
        if (!grenade)
        {
            Debug.LogWarning("GrenadeLauncher: échec instantiation grenade.");
            yield break;
        }
        
        grenade.SetDestination(targetPos);
        grenade.Fire();
        
        OnAttack?.Invoke();
        Reload();
    }

    public override void Reload()
    {
        StartCoroutine(ReloadCooldown());
    }

    private IEnumerator ReloadCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_ReloadTime);
        m_CanAttack = true;
        OnReload?.Invoke();
    }
}