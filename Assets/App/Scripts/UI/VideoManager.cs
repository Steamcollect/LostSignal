using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VideoManager : RegularSingleton<VideoManager>
{
    [Title("REFERENCES")]
    [SerializeField] private SSO_UniversalSettings m_Resolution;
    [SerializeField] private SSO_UniversalSettings m_WindowMode;

    private List<Resolution> m_Resolutions;

    protected override void Awake()
    {
        base.Awake();
        InitializeResolutions();

        m_Resolution.OnEnumChanged += SetResolution;

        m_WindowMode.OnEnumChanged += SetWindowMode;
    }

    private void OnDestroy()
    {
        m_Resolution.OnEnumChanged -= SetResolution;
    }

    private void InitializeResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        m_Resolutions = new List<Resolution>();

        double currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        for(int i = 0; i < allResolutions.Length; i++)
        {
            if(!m_Resolutions.Any(r => r.width == allResolutions[i].width && r.height == allResolutions[i].height))
            {
                m_Resolutions.Add(allResolutions[i]);
            }
        }

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < m_Resolutions.Count; i++)
        {
            string option = m_Resolutions[i].width + " x " + m_Resolutions[i].height;
            options.Add(option);

            if (m_Resolutions[i].width == Screen.width &&
                m_Resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        m_Resolution.EnumOptions = options.ToArray();

        m_Resolution.SetNewEnumValue(currentResolutionIndex);
    }

    private void SetResolution(int index)
    {
        if (index < 0 || index >= m_Resolutions.Count) return;

        Resolution resolution = m_Resolutions[index];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    private void SetWindowMode(int index)
    {
        switch (index)
        {
            case 0:
                WindowWindowed();
                break;
            case 1:
                WindowBorderless();
                break;
            case 2:
                WindowFullscreen();
                break;
        }
    }

    public void AnisotropicFilteringEnable()
    {
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
    }

    public void AnisotropicFilteringDisable()
    {
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
    }

    public void AntiAliasingSet(int index)
    {
        // 0, 2, 4, 8 - Zero means off
        QualitySettings.antiAliasing = index;
    }

    public void VsyncSet(int index)
    {
        // 0, 1 - Zero means off
        QualitySettings.vSyncCount = index;
    }

    public void ShadowResolutionSet(int index)
    {
        if (index == 3)
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
        else if (index == 2)
            QualitySettings.shadowResolution = ShadowResolution.High;
        else if (index == 1)
            QualitySettings.shadowResolution = ShadowResolution.Medium;
        else if (index == 0)
            QualitySettings.shadowResolution = ShadowResolution.Low;
    }

    public void ShadowsSet(int index)
    {
        if (index == 0)
            QualitySettings.shadows = ShadowQuality.Disable;
        else if (index == 1)
            QualitySettings.shadows = ShadowQuality.All;
    }

    public void ShadowsCascasedSet(int index)
    {
        //0 = No, 2 = Two, 4 = Four
        QualitySettings.shadowCascades = index;
    }

    public void TextureSet(int index)
    {
        // 0 = Full, 4 = Eight Resolution
        QualitySettings.globalTextureMipmapLimit = index;
    }

    public void SoftParticleSet(int index)
    {
        if (index == 0)
            QualitySettings.softParticles = false;
        else if (index == 1)
            QualitySettings.softParticles = true;
    }

    public void ReflectionSet(int index)
    {
        if (index == 0)
            QualitySettings.realtimeReflectionProbes = false;
        else if (index == 1)
            QualitySettings.realtimeReflectionProbes = true;
    }

    public void SetOverallQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void WindowFullscreen()
    {
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    public void WindowBorderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void WindowWindowed()
    {
        Screen.fullScreen = false;
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }
}