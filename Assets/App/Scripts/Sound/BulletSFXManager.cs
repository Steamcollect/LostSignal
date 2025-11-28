using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class BulletSFXManager : MonoBehaviour
{
    [SerializeField] private EventReference m_BulletImpactOnWallSFX;
    [SerializeField] private EventReference m_BulletImpactOnTargetSFX;
    
    private EventInstance m_BulletImpactOnWallSFXInstance;
    private EventInstance m_BulletImpactOnTargetSFXInstance;
    
    [SerializeField] private float m_ImpactOnWallVolume = 1.0f;
    [SerializeField] private float m_ImpactOnTargetVolume = 1.0f;
    
    [SerializeField] private float m_ImpactPitchVariance = 0.2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!m_BulletImpactOnWallSFX.IsNull)
        {
            m_BulletImpactOnWallSFXInstance = FMODUnity.RuntimeManager.CreateInstance(m_BulletImpactOnWallSFX);
            m_BulletImpactOnWallSFXInstance.setVolume(m_ImpactOnWallVolume);
        }
        
        if (!m_BulletImpactOnTargetSFX.IsNull)
        {
            m_BulletImpactOnTargetSFXInstance = FMODUnity.RuntimeManager.CreateInstance(m_BulletImpactOnTargetSFX);
            m_BulletImpactOnTargetSFXInstance.setVolume(m_ImpactOnTargetVolume);
        }
    }

    public void PlayImpactOnWall()
    {
        if (m_BulletImpactOnWallSFXInstance.isValid())
        {
            m_BulletImpactOnWallSFXInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            m_BulletImpactOnWallSFXInstance.setPitch(Random.value * m_ImpactPitchVariance + (1 - m_ImpactPitchVariance / 2));
            m_BulletImpactOnWallSFXInstance.start();
        }
    }
    
    public void PlayImpactOnTarget()
    {
        if (m_BulletImpactOnTargetSFXInstance.isValid())
        {
            m_BulletImpactOnTargetSFXInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            m_BulletImpactOnTargetSFXInstance.setPitch(Random.value * m_ImpactPitchVariance + (1 - m_ImpactPitchVariance / 2));
            m_BulletImpactOnTargetSFXInstance.start();
        }
    }
}
