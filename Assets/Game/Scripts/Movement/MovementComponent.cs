using Game.Scripts.Runtime.Data;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour
{
    //Movement Property
    [Header("Movement Property")]
    [field: SerializeField]
    public float MoveSpeed { get; set; }

    [field: SerializeField] public float MaxMoveSpeed { get; set; }
    [field: SerializeField] public float DampingMove { get; set; } = 10f;
    [field: SerializeField] public float DashForce { get; set; } = 10f;

    [Header("Jump Property")]
    //Jump Property
    [SerializeField]
    private int maxJumpCount = 2;

    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravityMultiplier = -5.25f;
    [field: SerializeField] public bool shouldUseGravity = true;


    //Motion Movement
    private Vector3 _horizontalMovement;
    private Vector3 _additionalMovement;
    private Vector3 _verticalMovement;
    private bool _movementInputPerformed = false;
    private int _jumpCount = 0;
    private float _xRotation;

    //Components
    [HideInInspector] public CharacterController CharacterController { get; private set; }
    private Camera _mainCamera;
    private float CamYLoc;

    public bool IsGrounded => CharacterController.isGrounded;


    public int MaxJumpCount
    {
        get => maxJumpCount;
        set => maxJumpCount = Mathf.Max(1, Mathf.Abs(value));
    }

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        if (!_mainCamera)
        {
            print("No camera found !");
            return;
        }

        CharacterController.enableOverlapRecovery = true;

        CamYLoc = _mainCamera.transform.position.y;
    }

    private void FixedUpdate()
    {
        // PerformMovement();
        // if (_mainCamera)
        // {
        //     Vector3 pos = transform.position;
        //     Vector3 targetPos = new Vector3(pos.x, CamYLoc, pos.z -2.5F);
        //     _mainCamera.transform.position =
        //         Vector3.Lerp(_mainCamera.transform.position, targetPos, 10f * Time.deltaTime);
        // }
    }


    public void ApplyMovement()
    {
        if (shouldUseGravity && !IsGrounded)
        {
            _verticalMovement -= Physics.gravity * (gravityMultiplier * Time.fixedDeltaTime);
        }

        if (!_movementInputPerformed)
        {
            _horizontalMovement = Vector3.Lerp(
                _horizontalMovement,
                Vector3.zero,
                Mathf.Max(1, DampingMove) * Time.fixedDeltaTime
            );
        }

        if (_additionalMovement.sqrMagnitude > 0)
        {
            _additionalMovement = Vector3.Lerp(
                _additionalMovement,
                Vector3.zero,
                Mathf.Max(1, DampingMove) * Time.fixedDeltaTime
            );
        }

        Vector3 finalMovement = /* x , z */_horizontalMovement + /* Y */_verticalMovement + _additionalMovement;
        CharacterController.Move(finalMovement * Time.fixedDeltaTime);

        //let move before trying to reset any data, could cause to movement issue at this point...
        float gravityFlag = -2f;
        if (CharacterController.isGrounded && !Mathf.Approximately(_verticalMovement.y, gravityFlag) && _jumpCount > 0)
        {
            _verticalMovement.y = gravityFlag; // -2f to ensure we have a grounded gravity apply
            _jumpCount = 0;
        }
    }

    public void MoveWithCam(Vector3 input)
    {
        _movementInputPerformed = input.sqrMagnitude > 0.01f;

        if (!_movementInputPerformed)
        {
            return;
        }

        // --- 1. Get camera-relative directions ---
        Vector3 camForward = _mainCamera.transform.forward;
        Vector3 camRight = _mainCamera.transform.right;

        // Flatten on XZ plane (ignore camera tilt)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // --- 2. Build world-space move direction ---
        Vector3 moveDir = camRight * input.x + camForward * input.z;

        Vector3 targetVelocity = moveDir.normalized * MoveSpeed;
        targetVelocity.y = 0; // not your job to preserve gravity/jump
        // --- 3. Apply final Movement ---
        _horizontalMovement = targetVelocity;

        Vector3 flatVel = new Vector3(_horizontalMovement.x, 0, _horizontalMovement.z);
        float currentSpeed = flatVel.magnitude;

        if (currentSpeed > MaxMoveSpeed)
        {
            // Scaling instead of overriding stuff
            float scale = MaxMoveSpeed / currentSpeed;
            _horizontalMovement *= scale;
        }

        // transform.Rotate(Vector3.up * moveDir.x);
    }

    public void Move(Vector3 input)
    {
        _movementInputPerformed = input.sqrMagnitude > 0.01f;

        if (!_movementInputPerformed)
        {
            return;
        }

        Vector3 moveDir = input.normalized;

        Vector3 targetVelocity = moveDir * MoveSpeed;
        targetVelocity.y = 0;

        _horizontalMovement = targetVelocity;

        Vector3 flatVel = new Vector3(_horizontalMovement.x, 0, _horizontalMovement.z);
        float currentSpeed = flatVel.magnitude;

        if (currentSpeed > MaxMoveSpeed)
        {
            // Scaling instead of overriding stuff
            float scale = MaxMoveSpeed / currentSpeed;
            _horizontalMovement *= scale;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir),
            0.5f);
        // transform.rotation = Quaternion.LookRotation(moveDir);
    }

    public void MoveWithClick()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // var x = Agent.SetDestination(hit.point);
        }
    }

    public void Look(Vector2 input, bool allowPitch = false)
    {
        if (input.magnitude <= 0.1f)
        {
            return;
        }

        Vector2 finalInput = input * (PlayerSetting.Sensitivity * Time.fixedDeltaTime);

        if (allowPitch)
        {
            // --- Vertical look (pitch) ---
            _xRotation -= finalInput.y;
            _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);
            _mainCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f,0f);
        }

        // --- Horizontal look (yaw) ---
        transform.Rotate(Vector3.up * finalInput.x);
    }

    public bool CanJump()
    {
        return _jumpCount < maxJumpCount;
    }

    public void Jump()
    {
        if (CanJump())
        {
            float jumpMultiplier = (_jumpCount == 0) ? 1f : 1.2f;
            _verticalMovement.y = jumpForce * jumpMultiplier;
            _jumpCount++;
        }
    }

    public void Dash()
    {
        Vector3 direction = new Vector3(_horizontalMovement.x, 0, _horizontalMovement.z);

        if (direction.sqrMagnitude < 0.2f)
        {
            direction = transform.forward;
        }

        _additionalMovement = direction.normalized * DashForce;
    }
}