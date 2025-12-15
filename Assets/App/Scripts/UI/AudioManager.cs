using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class audioManager : RegularSingleton<audioManager>
{
    [Title("AUDIO")]
    [SerializeField] private string m_MasterBusName;
    [SerializeField] private string m_MusicBusName;

    [Title("REFERENCES")]
    [SerializeField] private SSO_UniversalSettings m_MasterVolumeSetting;
    [SerializeField] private SSO_UniversalSettings m_MusicVolumeSetting;

    private FMOD.Studio.Bus m_MasterBus;
    private FMOD.Studio.Bus m_MusicBus;

    protected override void Awake()
    {
        base.Awake();

        InitializeAudio();
    }

    private void InitializeAudio()
    {
        // AUDIO
        //m_MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/+" + m_MasterBusName);
        //m_MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/+" + m_MusicBusName);
        //m_EffectsBus = FMODUnity.RuntimeManager.GetBus("bus:/+" + m_EffectsBusName);

        m_MasterBus.setVolume(m_MasterVolumeSetting.CurrentFloat);
        m_MasterBus.setVolume(m_MusicVolumeSetting.CurrentFloat);
    }

    public void VolumeSetMaster(float volume)
    {
        m_MasterBus.setVolume(volume);
    }

    public void VolumeSetMusic(float volume)
    {
        m_MusicBus.setVolume(volume);
    }
}