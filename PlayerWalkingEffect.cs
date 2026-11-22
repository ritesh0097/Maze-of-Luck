using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerWalkingEffect : MonoBehaviour
{
    [Header("References")]
    public Transform povCam;

    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float sprintSpeed = 1f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Look Settings")]
    [Tooltip("Sensitivity is much lower in the New Input System because it reads raw pixel delta.")]
    public float mouseSensitivity = 0.1f;
    public float minLookAngle = -85f;
    public float maxLookAngle = 85f;

    [Header("Head Bob Settings")]
    public bool headbobEnabled = true;
    public float stepFrequency = 2.4f;
    public float stepAmplitude = 0.18f;
    public float idleSwaySpeed = 1.5f;
    public float idleSwayAmp = 0.04f;
    public float leanIntensity = 0.08f;

    // Internal state
    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch = 0f;
    private bool cursorLocked = true;

    // Head-bob internal variables
    private float tBob;
    private float idleSwayT;
    private float bobIntensity;
    private float leanValue;
    private Vector3 cameraBaseLocalPosition;
    private Quaternion cameraBaseLocalRotation;

    // Self-contained Input Actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction escapeAction;
    private InputAction leftClickAction;


    // Sounds
    [Header("Sounds")]
    [SerializeField] AudioClip walkingSound;
    AudioSource audioSource;



    void Awake()
    {
        // Define inputs directly in code to avoid requiring an external Input Action Asset
        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        lookAction = new InputAction("Look", binding: "<Pointer>/delta");
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        sprintAction = new InputAction("Sprint", binding: "<Keyboard>/leftShift");
        escapeAction = new InputAction("Escape", binding: "<Keyboard>/escape");
        leftClickAction = new InputAction("LeftClick", binding: "<Mouse>/leftButton");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        escapeAction.Enable();
        leftClickAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        escapeAction.Disable();
        leftClickAction.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (povCam != null)
        {
            cameraBaseLocalPosition = povCam.localPosition;
            cameraBaseLocalRotation = povCam.localRotation;
        }

        SetCursorState(true);
    }

    void Update()
    {
        HandleCursorState();

        if (cursorLocked)
        {
            HandleMouseLook();
            HandleMovementAndJump();
            HandleHeadBob();
        }
        else
        {
            ApplyGravityOnly();
        }
    }

    private void HandleMouseLook()
    {
        Vector2 lookDelta = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        transform.Rotate(Vector3.up * lookDelta.x);

        cameraPitch -= lookDelta.y;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);

        if (povCam != null)
        {
            povCam.localRotation = cameraBaseLocalRotation * Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovementAndJump()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        movement = Vector3.ClampMagnitude(movement, 1f);

        bool isSprinting = sprintAction.IsPressed();
        float speed = 5f;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;

        }


        if (controller.isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = movement * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void ApplyGravityOnly()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleHeadBob()
    {
        if (povCam == null) return;

        float delta = Time.deltaTime;

        if (!headbobEnabled)
        {
            bobIntensity = Mathf.Lerp(bobIntensity, 0f, Mathf.Clamp01(8f * delta));
            leanValue = Mathf.Lerp(leanValue, 0f, Mathf.Clamp01(8f * delta));
            idleSwayT += delta * idleSwaySpeed;

            Vector3 disabledPosition = cameraBaseLocalPosition;
            disabledPosition.y += Mathf.Sin(idleSwayT) * idleSwayAmp * 0.35f;
            disabledPosition.x += Mathf.Cos(idleSwayT * 0.5f) * idleSwayAmp * 0.2f;

            povCam.localPosition = Vector3.Lerp(povCam.localPosition, disabledPosition, Mathf.Clamp01(8f * delta));
            ApplyCameraRoll(0f, delta);

        }


        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;
        float horizontalSpeed = horizontalVelocity.magnitude;
        bool isGrounded = controller.isGrounded;
        bool isSprinting = sprintAction.IsPressed();

        float targetBobIntensity = 0f;

        if (isGrounded && horizontalSpeed > 0.35f)
        {
            targetBobIntensity = Mathf.Clamp(horizontalSpeed / walkSpeed, 0f, 1f);
            if (isSprinting)
            {
                targetBobIntensity = Mathf.Clamp(targetBobIntensity * 1.08f, 0f, 1.15f);
            }

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(walkingSound);
            }
        }
        
        else
        {
            audioSource.Stop();
        }

        if (controller.collisionFlags.HasFlag(CollisionFlags.Sides) && horizontalSpeed < 0.45f)
        {
            targetBobIntensity = 0f;
        }

        float bobBlendSpeed = targetBobIntensity > bobIntensity ? 7f : 10f;
        bobIntensity = Mathf.Lerp(bobIntensity, targetBobIntensity, Mathf.Clamp01(bobBlendSpeed * delta));

        Vector3 targetPosition = cameraBaseLocalPosition;
        float targetRoll = 0f;

        if (bobIntensity > 0.01f)
        {
            tBob += delta * horizontalSpeed * stepFrequency;
            targetPosition.y += Mathf.Sin(tBob) * stepAmplitude * bobIntensity;
            targetPosition.x += Mathf.Cos(tBob * 0.5f) * (stepAmplitude * 0.4f) * bobIntensity;

            idleSwayT += delta * idleSwaySpeed;
            float idleAmount = Mathf.Clamp01(1f - bobIntensity);
            targetPosition.y += Mathf.Sin(idleSwayT) * idleSwayAmp * idleAmount;
            targetPosition.x += Mathf.Cos(idleSwayT * 0.5f) * (idleSwayAmp * 0.5f) * idleAmount;
        }
        else
        {
            idleSwayT += delta * idleSwaySpeed;
            targetPosition.y += Mathf.Sin(idleSwayT) * idleSwayAmp;
            targetPosition.x += Mathf.Cos(idleSwayT * 0.5f) * (idleSwayAmp * 0.5f);
        }

        float sideMove = Vector3.Dot(horizontalVelocity.normalized, transform.right);
        if (horizontalSpeed > 0.35f && isGrounded)
        {
            targetRoll = -sideMove * leanIntensity;
        }

        if (controller.collisionFlags.HasFlag(CollisionFlags.Sides) && horizontalSpeed < 0.45f)
        {
            targetRoll = 0f;
        }

        leanValue = Mathf.Lerp(leanValue, targetRoll, Mathf.Clamp01(7f * delta));

        povCam.localPosition = Vector3.Lerp(povCam.localPosition, targetPosition, Mathf.Clamp01(10f * delta));
        ApplyCameraRoll(leanValue, delta);
    }

    private void ApplyCameraRoll(float targetRoll, float delta)
    {
        Quaternion targetRotation = cameraBaseLocalRotation * Quaternion.Euler(cameraPitch, 0f, targetRoll);
        povCam.localRotation = Quaternion.Slerp(povCam.localRotation, targetRotation, Mathf.Clamp01(8f * delta));
    }

    private void HandleCursorState()
    {
        if (escapeAction.WasPressedThisFrame())
        {
            SetCursorState(false);
        }

        if (!cursorLocked && leftClickAction.WasPressedThisFrame())
        {
            SetCursorState(true);
        }
    }

    private void SetCursorState(bool locked)
    {
        cursorLocked = locked;
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}