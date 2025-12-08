using System;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour
{
    [SerializeField] private RSO_Mana currentMana;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] Slider manaSlider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMana.Set(maxMana);
    }

    private void OnEnable()
    {
        currentMana.OnChanged += UpdateManaUI;
    }
    
    private void UpdateManaUI(float _)
    {
        manaSlider.value = currentMana.Get() / maxMana;
    }
}
