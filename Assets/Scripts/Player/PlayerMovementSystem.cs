using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementSystem : MonoBehaviour
{
    public static PlayerMovementSystem Instance;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private Camera bodyCamera = null;
    [SerializeField] private bool invertCamera = false;
    [SerializeField] private float cameraLerpSpeed = 20f;
    [Range(30f, 170f)][SerializeField] private float initFOV = 90f;
    [Range(40f, 90f)][SerializeField] private float maxLookAngle = 70f;



    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private bool holdToZoom = false;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float zoomStepTime = 5f;



    [Header("Sprint")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float sprintFOV = 80f;
    [SerializeField] private float sprintStaminaDrain = 0.1f;
    [SerializeField] private float sprintFOVStepTime = 10f;


    
    [Header("Gravity")]
    [SerializeField] private float gravity = -13.0f;
    [SerializeField] private float groundCheckRange = 0.75f;
    [SerializeField] private float crouchGroundCheckRange = 0.75f;



    [Header("Jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpStaminaDrain = 15f;



    [Header("Crouch")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private float crouchHeight = .75f;
    [SerializeField] private float speedReduction = .5f;


    [Header("Head Bob")]
    [SerializeField] private Transform joint = null;
    [SerializeField] private float bobSpeed = 10f;
    [SerializeField] private Vector3 bobAmount = new(0f, .1f, 0f);



    [Header("Initial Values")]
    [Range(0.1f, 10f)][SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float moveSmoothTime = 0.30f;



    [Header("Ceiling Test")]
    [SerializeField] private float castRadius = 0.5f;
    [SerializeField] private float castDistance = 1.5f;
    [SerializeField] private LayerMask layerMask = 1;



    [Header("===Components===")]
    public StaminaControl staminaControl = null;



    [Header("===PlayerMovement===")]
    [SerializeField] private bool autoWalk = false;
    [Space]
    [SerializeField] private bool cantMove = false;
    [SerializeField] private bool cantLook = false;
    [SerializeField] private bool cantJump = false;
    [SerializeField] private bool cantSprint = false;
    [SerializeField] private bool cantCrouch = false;

    //=============================================================

    // Movement Base
    private float speed = 0f;
    private Vector2 targetDir = Vector2.zero;
    private CharacterController controller = null;
    private Vector2 currentDirVelocity = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector2 currentDir = Vector2.zero;


    // Camera Rotation
    private float pitch = 0.0f;
    private float yaw = 0.0f;


    // Zoom
    private bool isZoomed = false;

    public UnityAction OnLanding;

    // Movement Debug
    [HideInInspector] public float velocityY = 0f;
    public bool isJumping = false;
    [HideInInspector] public bool isWalking = false;
     public bool isGrounded = false;
    [HideInInspector] public bool isCrouched = false;
    [HideInInspector] public bool isSprinting = false;

    // GroudCheck
    private bool wasGrounded;
    private float originalGroundCheckRange = 0.75f;

    //Slop
    private float originalHeight  = 2f;
    private float slopeForce = 5.0f;
    private float slopeForceRayLength = 2.0f;

    // Head Bob
    private Vector3 jointOriginalPos;
    private float timer = 0;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        playerCamera.fieldOfView = initFOV;
        //bodyCamera.fieldOfView = initFOV;
        originalHeight = controller.height;
        jointOriginalPos = joint.localPosition;
        originalGroundCheckRange = groundCheckRange;

        Instance = this;

        speed = walkSpeed;
    }

    private void Update()
    {
        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), autoWalk? 1f : Input.GetAxisRaw("Vertical"));

        MouseLook();
        ZoomControl();
        Jump();

        if (wasGrounded != CheckGround())
        {
            GroundedChanged(CheckGround());
            wasGrounded = CheckGround();
        }
    }

    private void FixedUpdate()
    {
        Movement();
        Sprint();
        Crouch();
        HeadBob();

        //bodyCamera.fieldOfView = playerCamera.fieldOfView;
    }

    private void MouseLook()
    {
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, joint.transform.position, cameraLerpSpeed * Time.deltaTime);

        if (cantLook)
            return;

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

        if (!invertCamera)
            pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
        else
            pitch += mouseSensitivity * Input.GetAxis("Mouse Y");

        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = new Vector3(0f, yaw, 0f);
        //bodyCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void Movement()
    {
        velocityY += gravity * Time.deltaTime;

        if (cantMove)
        {
            controller.Move(Vector3.up * velocityY * Time.deltaTime);
            return;
        }

        isWalking = targetDir.x != 0 || targetDir.y != 0 && CheckGround();

        targetDir.Normalize();
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (CheckGround() && !isJumping)
            velocityY = 0;

        velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * speed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if ((Mathf.Abs(targetDir.x) > 0 || Mathf.Abs(targetDir.y) > 0) && OnSlope())
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
    }

    private void ZoomControl()
    {
        if (!enableZoom)
            return;

        if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
        {
            if (!isZoomed)
                isZoomed = true;
            else
                isZoomed = false;
        }

        if (holdToZoom && !isSprinting)
        {
            if (Input.GetKeyDown(zoomKey))
                isZoomed = true;
            else if (Input.GetKeyUp(zoomKey))
                isZoomed = false;
        }

        if (isZoomed)
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
        else if (!isZoomed && !isSprinting)
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initFOV, zoomStepTime * Time.deltaTime);
    }

    private void Sprint()
    {
        if (cantSprint || isCrouched)
        {
            if (isSprinting)
            {
                speed = walkSpeed;
                isSprinting = false;
            }
            return;
        }

        if (staminaControl.tryingSprint = Input.GetKey(sprintKey))
        {
            if (isSprinting = staminaControl.UseStamina(sprintStaminaDrain))
                speed = sprintSpeed;
            else
                speed = walkSpeed;
        }

        if (isSprinting)
        {
            isZoomed = false;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

            if (!Input.GetKey(sprintKey))
            {
                speed = walkSpeed;
                isSprinting = false;
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(jumpKey) && CheckGround() && !cantJump && !isCrouched)
        {
            if(staminaControl.UseStamina(jumpStaminaDrain))
            {
                isJumping = true;
                velocityY = jumpForce;
                StartCoroutine(BackToGround());
            }
        }
    }

    private void Crouch()
    {
        if (cantCrouch)
            return;

        if (Input.GetKey(crouchKey) && !isCrouched)
        {
            controller.height = crouchHeight;
            groundCheckRange = crouchGroundCheckRange;
            isCrouched = true;
        }

        if (isCrouched && !Input.GetKey(crouchKey))
        {
            if (!CheckCeilingHeight())
            {
                controller.height = originalHeight;
                groundCheckRange = originalGroundCheckRange;
                controller.Move(new Vector3(0.0f,-0.01f,0f));

                isCrouched = false;
            }
        }
    }

    private void HeadBob()
    {
        if (isWalking)
        {
            if (isSprinting)
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            else if (isCrouched)
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            else
                timer += Time.deltaTime * bobSpeed;

            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }

    public void LockPlayer(bool _lock)
    {
        cantMove = _lock;
        cantLook = _lock;
        cantJump = _lock;
        cantSprint = _lock;
        cantCrouch = _lock;
    }

    public void LockVision(bool _lock)
    {
        cantLook = _lock;
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;

        return false;
    }

    private bool CheckGround()
    {
        return Physics.SphereCast(transform.position, castRadius, -transform.up, out RaycastHit hit, groundCheckRange, layerMask, QueryTriggerInteraction.Ignore);
    }

    void GroundedChanged(bool state)
    {
        if (state)
        {
            //print("velocityY: " + velocityY);
            isGrounded = true;
            OnLanding?.Invoke();
        }
        else
            isGrounded = false;
    }

    private bool CheckCeilingHeight()
    {
        return Physics.SphereCast(transform.position, castRadius, transform.up, out RaycastHit hit, castDistance, layerMask, QueryTriggerInteraction.Ignore);
    }

    private IEnumerator BackToGround()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.3f);
        yield return wfs;
        isJumping = false;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position - transform.up * groundCheckRange, castRadius);
        // Gizmos.DrawSphere(transform.position + transform.up * castDistance, castRadius);
    }
}