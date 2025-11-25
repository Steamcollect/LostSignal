using UnityEngine;
using MVsToolkit.Dev;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //[Header("Settings")]
    
    [Header("References")]
    [SerializeField] InterfaceReference<IMovement> movement;

    [Header("Input")]
    [SerializeField] InputActionReference moveInputAction;
    [SerializeField] RSO_MainCamera mainCamera;
    
    //[Header("Output")]

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveInputAction.action.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude <= .1f) return;

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + (mainCamera.Get().transform.eulerAngles.y);
        Vector3 moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        movement.Value.Move(moveDir);
    }
}