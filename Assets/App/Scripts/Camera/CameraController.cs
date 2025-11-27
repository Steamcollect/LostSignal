using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float timeOffset;
    [SerializeField] bool useStartPositionAsOffset;
    [SerializeField, HideIf("useStartPositionAsOffset", true)] Vector3 posOffset;

    Vector3 velocity;

    [Space(10)]
    [SerializeField] float maxExplorationDistance = 5;
    [SerializeField] AnimationCurve multPerDist;

    [Header("References")]
    [SerializeField] Camera cam;
    
    Transform target;

    [Header("Input")]
    [SerializeField] RSE_SetCameraTarget setTarget;
    [SerializeField] InputActionReference mousePosIA;

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

        Vector3 basePos = target.position + posOffset;
        Vector3 mousePos = GetMouseWorldPos();

        Vector3 dir = (mousePos - target.position).normalized;

        float dist01 = Mathf.Clamp(
            Vector3.Distance(target.position, mousePos),
            0, maxExplorationDistance
        ) / maxExplorationDistance;

        float mult = multPerDist.Evaluate(dist01);

        Vector3 explorationOffset = dir * maxExplorationDistance * mult;

        Vector3 finalPos = basePos + explorationOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPos,
            ref velocity,
            timeOffset
        );
    }

    Vector3 GetMouseWorldPos()
    {
        Ray ray = cam.ScreenPointToRay(mousePosIA .action.ReadValue<Vector2>());

        if (Mathf.Abs(ray.direction.y) < 0.0001f)
            return Vector3.zero;

        float t = (target.position.y - ray.origin.y) / ray.direction.y;

        return ray.origin + ray.direction * t;
    }

    void SetTarget(Transform target) { this.target = target; }

    public Camera GetCamera() { return cam; }
}