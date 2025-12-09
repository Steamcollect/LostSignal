using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Panel : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private Selectable m_FirstSelected = null;
    [SerializeField] private CanvasGroup m_CanvasGroup = null;
    [SerializeField] private Animator m_Animator = null;

    [Title("Settings")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "ButtonName")]
    [SerializeField] private List<ButtonItem> m_Buttons = new List<ButtonItem>();

    private string m_PanelFadeIn = "FadeIn";
    private string m_PanelFadeOut = "FadeOut";

    [System.Serializable]
    private class ButtonItem
    {
        public string ButtonName = "DefaultButton";
        public Button Button;
        public UnityEvent OnClick;
    }

    private void Awake()
    {
        if (m_Buttons.Count == 0) return;
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < m_Buttons.Count; i++)
        {
            if (m_Buttons[i].Button == null) continue;

            ButtonItem button = m_Buttons[i];
            m_Buttons[i].Button.onClick.AddListener(() => button.OnClick.Invoke());
        }
    }
    public void SetPanelActive(bool active)
    {
        if (m_CanvasGroup == null) return;

        if (active)
        {
            if(m_FirstSelected != null) m_FirstSelected.Select();

            m_Animator.SetTrigger(m_PanelFadeIn);
        }
        else
        {
            m_Animator.SetTrigger(m_PanelFadeOut);
        }
    }
}