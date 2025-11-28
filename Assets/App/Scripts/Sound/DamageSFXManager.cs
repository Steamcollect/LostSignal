using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class DamageSFXManager : MonoBehaviour
{
    [SerializeField] private EventReference m_DamageSFX;
    [SerializeField] private EventReference m_DeathSFX;
    
    [SerializeField] private float m_DamageVolume = 1.0f;
    [SerializeField] private float m_DeathVolume = 1.0f;
    
    [SerializeField] private float m_DamagePitchVariance = 0.2f;
    
    private EventInstance m_DamageSFXInstance;
    private EventInstance m_DeathSFXInstance;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!m_DamageSFX.IsNull)
        {
            m_DamageSFXInstance = FMODUnity.RuntimeManager.CreateInstance(m_DamageSFX);
            m_DamageSFXInstance.setVolume(m_DamageVolume);
        }
        
        if (!m_DeathSFX.IsNull)
        {
            m_DeathSFXInstance = FMODUnity.RuntimeManager.CreateInstance(m_DeathSFX);
            m_DeathSFXInstance.setVolume(m_DeathVolume);
        }
    }

    public void PlayDamageSFX()
    {
        if (m_DamageSFXInstance.isValid())
        {
            m_DamageSFXInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            m_DamageSFXInstance.setPitch(Random.value * m_DamagePitchVariance + (1 - m_DamagePitchVariance / 2));
            m_DamageSFXInstance.start();
        }
    }
    
    public void PlayDeathSFX()
    {
        if (m_DeathSFXInstance.isValid())
        {
            m_DeathSFXInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            m_DeathSFXInstance.start();
        }
    }
}
