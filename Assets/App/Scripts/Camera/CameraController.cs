using MVsToolkit.Dev;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float timeOffset;
    [SerializeField] bool useStartPositionAsOffset;
    [SerializeField, HideIf("useStartPositionAsOffset", true)] Vector3 posOffset;

    Vector3 velocity;

    [Header("References")]
    [SerializeField] Camera cam;
    
    Transform target;

    [Header("Input")]
    [SerializeField] RSE_SetCameraTarget setTarget;
    
    [Header("Output")]
    [SerializeField] RSO_MainCamera rso_MainCamera;

    private void OnEnable()
    {
        setTarget.Action += SetTarget;
    }
    private void OnDisable()
    {
        setTarget.Action -= SetTarget;
    }

    void Awake()
    {
        rso_MainCamera.Set(this);
    }

    private void Start()
    {
        if(useStartPositionAsOffset) posOffset = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, target.position + posOffset, ref velocity, timeOffset);
    }

    void SetTarget(Transform target) { this.target = target; }

    public Camera GetCamera() { return cam; }
}