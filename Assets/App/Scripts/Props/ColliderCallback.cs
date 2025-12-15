using System;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ColliderCallback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool m_UseTagFilter = false;
    [Sirenix.OdinInspector.ShowIf("m_UseTagFilter"),SerializeField, TagName] private string[] m_TagsToDetect;

    [Header("Output")]
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerExitEvent;
    
    public Action<Collider> OnTriggerEnterCallback;
    public Action<Collider> OnTriggerExitCallback;

    private void OnTriggerEnter(Collider other)
    {
        if (m_UseTagFilter && Array.IndexOf(m_TagsToDetect, other.tag) == -1) return;
        OnTriggerEnterCallback?.Invoke(other);
        OnTriggerEnterEvent?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_UseTagFilter && Array.IndexOf(m_TagsToDetect, other.tag) == -1) return;
        OnTriggerExitCallback?.Invoke(other);
        OnTriggerExitEvent?.Invoke();
    }
    
    
}