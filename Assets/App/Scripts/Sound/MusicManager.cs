using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    const float EXPLORATION_PHASE = 0f;
    const float BATTLE_PHASE = 1f;
    
    [SerializeField] private EventReference m_Music;
    private EventInstance m_MusicInstance;
    
    [SerializeField] private RSE_OnFightStarted m_FightStarted;
    [SerializeField] private RSE_OnFightEnded m_FightEnded;

    private void OnEnable()
    {
        m_FightStarted.Action += SwitchToBattle;
        m_FightEnded.Action += SwitchToExploration;
    }
    
    private void OnDisable()
    {
        m_FightStarted.Action -= SwitchToBattle;
        m_FightEnded.Action -= SwitchToExploration;
    }

    void Start()
    {
        m_MusicInstance = FMODUnity.RuntimeManager.CreateInstance(m_Music);
        SwitchToExploration();
        m_MusicInstance.start();
    }

    public void Play()
    {
        m_MusicInstance.start();
    }
    
    public void Stop()
    {
        m_MusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    
    private void SwitchToBattle()
    {
        m_MusicInstance.setParameterByName("Phase", BATTLE_PHASE);
    }
    
    private void SwitchToExploration()
    {
        m_MusicInstance.setParameterByName("Phase", EXPLORATION_PHASE);
    }
}
