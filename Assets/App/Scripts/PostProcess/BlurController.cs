using UnityEngine;

public class BlurController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Height = 10.0f;

    [Header("References")]
    [SerializeField] private Material m_FullScreenMaterial;
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private string m_ReferenceName = "_PlayerY";
    private int m_PlayerYID;

    private void Start()
    {
        if (m_FullScreenMaterial != null)
        {
            m_PlayerYID = Shader.PropertyToID(m_ReferenceName);
        }
        else
        {
            Debug.LogError("Attention : Aucun Material assigné au script BlurController !");
        }
    }

    void Update()
    {
        if (m_FullScreenMaterial != null && m_PlayerController != null)
        {
            m_FullScreenMaterial.SetFloat(m_PlayerYID, m_PlayerController.Get().GetTargetPosition().y - m_Height);
        }
    }
}