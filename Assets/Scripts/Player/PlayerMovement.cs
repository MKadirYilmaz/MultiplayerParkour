using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(PlayerManager), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    #region Attributes
    [Header("Setup")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask groundLayer;

    [Header("Adjustments")]
    [SerializeField] private float accelerationForce = 30f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float slidingMinSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int airJumpAmount = 1;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float groundCheckDist = 1f;
    [Range(0,1)][SerializeField] private float airControl = 0.5f;

    public delegate void JumpTrigger();
    public JumpTrigger OnJumpTrigger;

    private PlayerManager pManager;

    private Rigidbody rb;
    private InputSystem input;

    private Vector3 moveVector;
    public Vector2 moveInput {get; private set;}
    public Vector2 lookInput {get; private set;}

    private float xRotate = 0;
    private float yRotate = 0;

    private int currentAirJump = 0;
    private float targetMoveSpeed;
    private bool isPressingRun = false;
    private bool isPressingCrouch = false;
    public enum MovementState
    {
        Walking,
        Running,
        Air,
        Wallrunning,
        Sliding,
        Crouching
    }
    public MovementState moveState = MovementState.Walking;

    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        input = pManager.input;

        input.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>().normalized;
        input.Player.Move.canceled += context => moveInput = Vector2.zero;

        input.Player.Look.performed += context => lookInput = context.ReadValue<Vector2>();
        input.Player.Look.canceled += context => lookInput = Vector2.zero;

        input.Player.Jump.performed += context => Jump();

        input.Player.Sprint.performed += context => StartRun();
        input.Player.Sprint.canceled += context => EndRun();

        input.Player.Crouch.performed += context => StartCrouching();
        input.Player.Crouch.canceled += context => EndCrouching();

        targetMoveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        StateMachine();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void StateMachine()
    {
        if(IsGrounded())
        {
            currentAirJump = 0;
            // Crouching and sliding is priority
            if(isPressingCrouch)
            {
                moveState = CanSlide() ? MovementState.Sliding : MovementState.Crouching;
            }
            else
            {
                moveState = isPressingRun ? MovementState.Running : MovementState.Walking;
            }
        }
        if(!IsGrounded() && moveState != MovementState.Wallrunning)
        {
            moveState = MovementState.Air;
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        switch(moveState)
        {
            case MovementState.Walking:
                targetMoveSpeed = walkSpeed;
                moveVector = moveDirection * accelerationForce;
                rb.AddForce(moveVector, ForceMode.Force);
                break;
            case MovementState.Running:
                targetMoveSpeed = runSpeed;
                moveVector = moveDirection * accelerationForce;
                rb.AddForce(moveVector, ForceMode.Force);
                break;
            case MovementState.Air:
                moveVector = moveDirection * accelerationForce * airControl;
                rb.AddForce(moveVector, ForceMode.Force);
                break;
            case MovementState.Wallrunning:
                // Wallrun movement handled in WallrunSystem component
                break;
            case MovementState.Sliding:
                // Sliding movement
                break;
            case MovementState.Crouching:
                targetMoveSpeed = crouchSpeed;
                moveVector = moveDirection * accelerationForce;
                rb.AddForce(moveVector, ForceMode.Force);
                break;
            default:
                break;
        }
        
        
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        if(speed > targetMoveSpeed)
        {
            rb.linearVelocity = new Vector3((rb.linearVelocity.normalized * targetMoveSpeed).x, rb.linearVelocity.y, (rb.linearVelocity.normalized * targetMoveSpeed).z);
        }
    }

    private void StartRun()
    {
        isPressingRun = true;
        if(IsGrounded() && moveState != MovementState.Wallrunning)
        {
            moveState = MovementState.Running;
        }
    }

    private void EndRun()
    {
        isPressingRun = false;
        if(IsGrounded() && moveState != MovementState.Wallrunning)
        {
            moveState = MovementState.Walking;
        }
    }

    private void StartCrouching()
    {
        transform.localScale = new Vector3(1, 0.5f, 1);
        isPressingCrouch = true;
        if(CanSlide())
        {
            moveState = MovementState.Sliding;
        }
        else
        {
            moveState = MovementState.Crouching;
        }
    }

    private void EndCrouching()
    {
        
        Ray ray = new Ray(transform.position, transform.up);
        if(Physics.Raycast(ray, 1.1f, groundLayer))
        {
            
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            isPressingCrouch = false;
        }
    }

    private void Jump()
    {
        if(IsGrounded())
        {
            rb.AddForce(transform.up * jumpForce * rb.mass, ForceMode.Impulse);
            if(isPressingCrouch)
            {
                EndCrouching();
            }
        }
        else if(currentAirJump < airJumpAmount && moveState != MovementState.Wallrunning)
        {
            rb.AddForce(transform.up * jumpForce * rb.mass, ForceMode.Impulse);
            currentAirJump++;
        }
        OnJumpTrigger?.Invoke();
    }

    private void HandleRotation()
    {
        xRotate -= lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotate = Mathf.Clamp(xRotate, -90f, 90f);
        yRotate += lookInput.x * mouseSensitivity * Time.deltaTime;

        cameraTransform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
        transform.rotation = Quaternion.Euler(0, yRotate, 0);
        
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        return Physics.Raycast(ray, groundCheckDist, groundLayer);
    }

    private RaycastHit GetGroundHit()
    {
         Ray ray = new Ray(transform.position, -transform.up);
         Physics.Raycast(ray, out RaycastHit hitInfo, groundCheckDist, groundLayer);
         return hitInfo;
    }

    public Rigidbody GetRigidbody() => rb;
    public void SetGravityActive(bool isActive) => rb.useGravity = isActive;
    public bool GetIsGravityActive() => rb.useGravity;
    public float GetMoveSpeed() => targetMoveSpeed;
    public void ResetCurrentAirJumpCount() => currentAirJump = 0;
    public Vector3 GetHorizontalVelocity() => new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);


    private bool CanSlide() => GetHorizontalVelocity().magnitude > slidingMinSpeed;

}
