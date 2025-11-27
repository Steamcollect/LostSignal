using UnityEngine;

public class DamageVisual : MonoBehaviour
{
    private MeshRenderer[] m_Renderers;
    private MaterialPropertyBlock m_Block;

    void Awake()
    {
        m_Renderers = GetComponentsInChildren<MeshRenderer>();

        m_Block = new MaterialPropertyBlock();
    }

    private void Start()
    {
        SetDamage(0);
    }

    public void SetDamage(float value)
    {
        foreach (MeshRenderer renderer in m_Renderers)
        {
            renderer.GetPropertyBlock(m_Block);
            m_Block.SetFloat("_DamageAmount", value);
            renderer.SetPropertyBlock(m_Block);
        }
    }
}