using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;

public class TurretEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_AttackRange;
    [SerializeField, Range(1, 180)] float m_AngleRequireToAttack;

    [Space(10)]
    [SerializeField, ReadOnly] EnemyStates m_CurrentState;

    [Header("Internal References")]
    [SerializeField] PlayerDetector m_Detector;

    [Space(10)]
    [SerializeField] private RSO_PlayerController m_Player;

    private void FixedUpdate()
    {
        if (m_CurrentState == EnemyStates.Chasing)
        {
            if (m_Detector.CanSeePlayer(m_DetectionRange))
            {
                m_Combat.LookAt(m_Player.Get().GetTargetPosition());

                if (m_Detector.IsLookDirectionWithinAngle(GetTargetPosition(), m_Combat.GetLookAtDirection(), m_AngleRequireToAttack))
                    StartCoroutine(Attack());
            }
        }
        else if (m_CurrentState == EnemyStates.Idle
            && m_Detector.IsPlayerInRange(m_DetectionRange)
            && m_Detector.CanSeePlayer(m_DetectionRange))
        {
            m_CurrentState = EnemyStates.Chasing;
        }
    }

    IEnumerator Attack()
    {
        m_CurrentState = EnemyStates.Attacking;
        yield return StartCoroutine(m_Combat.Attack());
        m_CurrentState = EnemyStates.Chasing;
    }

    public void OnSpawn()
    {
        m_CurrentState = EnemyStates.Chasing;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);
    }
}