using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputComponent : MonoBehaviour
{
    private InputSystem_Actions _controls;

    // --- Public Actions ---
    public Action<Vector3> OnMoveAction;
    public Action<Vector2> OnLookAction;

    /*
     * True if the fire button is pressed down, false if released
     */
    public Action<bool> OnFireAction;


    public Action OnJumpAction;
    public Action OnDashAction;

    // --- Values ---
    private Vector3 _inputMovement;
    private Vector2 _lookInput;

    public Vector3 InputMovement => _inputMovement;
    public Vector3 InputLook => _lookInput;

    private bool _isProcessingMovement;

    private void Awake()
    {
        _controls = new InputSystem_Actions();
    }

    public void LockCursor(bool isCursorLocked, bool isCursorVisible = false)
    {
        Cursor.visible = true;
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
    }

    // --- Input Callbacks ---
    private void OnMoveStart(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _inputMovement.Set(input.x, 0, input.y);
        OnMoveAction?.Invoke(InputMovement);
    }

    private void OnMoveStop(InputAction.CallbackContext context)
    {
        _inputMovement = Vector3.zero;
        OnMoveAction?.Invoke(InputMovement);
    }

    private void OnLookPerform(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
        OnLookAction?.Invoke(_lookInput);
    }

    private void OnFirePerform(InputAction.CallbackContext context)
    {
        bool isHoldingFire = context.performed;
        OnFireAction?.Invoke(isHoldingFire);
    }


    private void OnJumpPerform(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpAction?.Invoke();
        }
    }

    private void OnDashPerform(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnDashAction?.Invoke();
        }
    }

    private void OnEnable()
    {
        _controls.Gameplay.Move.performed += OnMoveStart;
        _controls.Gameplay.Move.canceled += OnMoveStop;

        _controls.Gameplay.Look.performed += OnLookPerform;
        _controls.Gameplay.Look.canceled += OnLookPerform;


        _controls.Gameplay.Jump.performed += OnJumpPerform;
        _controls.Gameplay.Dash.performed += OnDashPerform;

        _controls.Gameplay.Fire.canceled += OnFirePerform;
        _controls.Gameplay.Fire.performed += OnFirePerform;


        LockCursor(true, false);
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Gameplay.Move.performed -= OnMoveStart;
        _controls.Gameplay.Move.canceled -= OnMoveStop;

        _controls.Gameplay.Look.performed -= OnLookPerform;
        _controls.Gameplay.Look.canceled -= OnLookPerform;

        _controls.Gameplay.Jump.performed -= OnJumpPerform;
        _controls.Gameplay.Dash.performed -= OnDashPerform;

        _controls.Gameplay.Fire.canceled -= OnFirePerform;
        _controls.Gameplay.Fire.performed -= OnFirePerform;

        LockCursor(false);
        _controls.Disable();
    }
}