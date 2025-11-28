using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class RangeReloadingWeaponSFXManager : MonoBehaviour
{
    [SerializeField] private EventReference m_FireSFX;
    [SerializeField] private EventReference m_ReloadSFX;
    private EventInstance m_FireSFXInstance;
    private EventInstance m_ReloadSFXInstance;
    
    [SerializeField] private float m_FirePitchVariance = 0.2f;
    [SerializeField] private float m_ReloadVolume = 1.0f;
    [SerializeField] private float m_FireVolume = 1.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!m_FireSFX.IsNull)
        {
            m_FireSFXInstance = FMODUnity.RuntimeManager.CreateInstance(m_FireSFX);
            m_FireSFXInstance.setVolume(m_FireVolume);
        }

        if (!m_ReloadSFX.IsNull)
        {
            m_ReloadSFXInstance = FMODUnity.RuntimeManager.CreateInstance(m_ReloadSFX);
            m_ReloadSFXInstance.setVolume(m_ReloadVolume);
        }
    }
    
    [Button("Attack SFX")]
    public void PlayAttackSFX()
    {
        if (!m_FireSFXInstance.isValid()) return;
        
        m_FireSFXInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        m_FireSFXInstance.setPitch(Random.value * m_FirePitchVariance + (1 - m_FirePitchVariance / 2));
        m_FireSFXInstance.start();
    }

    [Button("Reload SFX")]
    public void PlayReloadSFX()
    {
        if (!m_ReloadSFXInstance.isValid()) return;
        
        m_ReloadSFXInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        m_ReloadSFXInstance.start();
    }
}
