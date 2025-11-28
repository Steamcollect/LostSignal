using UnityEngine;

public class EntityRotationVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotationTime;
    [SerializeField] float maxRotationAngle;

    Vector3 currentRot;
    Vector3 rotationVelocity;

    [Header("References")]
    [SerializeField] Transform rotationPivot;
    
    //[Header("Input")]
    //[Header("Output")]

    public void Rotate(Vector3 input)
    {
        Vector3 target = new Vector3(input.z, 0, -input.x) * maxRotationAngle;

        currentRot = Vector3.SmoothDamp(currentRot, target, ref rotationVelocity, rotationTime);
        rotationPivot.localEulerAngles = currentRot;
    }
}