using System;
using UnityEngine;

public class RangeOutlineBarricade : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] int m_DefaultLayer;
    [SerializeField] int m_OutlineLayer;
    
    [Header("References")]
    [SerializeField] private Barricade m_Barricade;
    [SerializeField] private GameObject[] m_OutlineObjects;
    [Space(10)]
    [SerializeField] private RSO_PlayerController m_PlayerController;
    
    private bool m_OutlineEnabled;
    
    private void LateUpdate()
    {
        bool outlineEnabled = m_Barricade.IsInRange(m_PlayerController.Get().GetTargetPosition());
        if (m_OutlineEnabled == outlineEnabled) return;
        m_OutlineEnabled = outlineEnabled;
        SetOutline(m_OutlineEnabled);
    }

    private void SetOutline(bool outlineEnabled)
    {
        foreach (GameObject outlineObject in m_OutlineObjects)
        {
            outlineObject.layer = outlineEnabled ? m_OutlineLayer : m_DefaultLayer;
        }
    }
}
