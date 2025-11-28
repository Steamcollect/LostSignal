using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    [SerializeField] private EventReference m_Ambiance;
    private EventInstance m_MusicInstance;
    
    void Start()
    {
        m_MusicInstance = FMODUnity.RuntimeManager.CreateInstance(m_Ambiance);
        m_MusicInstance.start();
    }
}
