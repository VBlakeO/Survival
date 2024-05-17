using UnityEngine.Animations;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement m_Instance;

    #region Public variables
    [Header("===PlayerMovement===")]
    public bool m_CantMove;
    public bool m_CantLook;
    public bool m_CantJump;
    public float m_MouseSensitivity;

    [Header("===Speed===")]
    public float m_WalkingSpeed = 10.0f;
    public float m_RunningSpeed = 15.0f;

    [Header("===Gravity===")]
    public float m_Gravity = -13.0f;
    public float m_JumpForce = 8.00f;
    public float m_MoveSmoothTime = 0.30f;

    [Header("===Components===")]
    public Camera m_CameraFps;
    public Transform m_Head;

    public Vector2 m_CurrentDir = Vector2.zero;

    [SerializeField] private InventoryController inventoryController;
    #endregion

    #region Private variables
    private bool isJumping;

    private float w;
    private float r;
    private float headPitch = 0.0f;
    private float velocityY = 0.0f;
    private float speed = 0.0f;
    private float slopeForce = 5.0f, slopeForceRayLength = 2.0f;
    Vector3 velocity;

    public Vector2 targetMouseDelta = Vector2.zero;
    private Vector2 targetDir = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    [HideInInspector] public CharacterController m_Controller;
    #endregion

    private void Awake()
    {
        m_Instance = this;
        Initialize();
    }

    public void Initialize()
    {
        w = m_WalkingSpeed;
        r = m_RunningSpeed;
        speed = m_WalkingSpeed;

        m_Controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        inventoryController.OnBackpackStateChange += FreezePlayerCamera;
        inventoryController.OnExternalInventoryOpen += FreezePlayer;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if ((Input.GetKeyDown(KeyCode.Space)) && m_Controller.isGrounded && !m_CantJump)
            Jump();

        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 p1 = transform.position + m_Controller.center;
    }

    private void LateUpdate()
    {
        MouseLook();
        if (Time.timeScale == 0)
            return;

    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        Running();
        Movement();
    }

    void MouseLook()
    {
        if (m_CantLook)
            return;

        targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        headPitch -= targetMouseDelta.y * m_MouseSensitivity;
        headPitch = Mathf.Clamp(headPitch, -85, 75);

        m_Head.localEulerAngles = Vector3.right * headPitch;
        transform.Rotate(Vector3.up * (targetMouseDelta.x) * m_MouseSensitivity);
    }

    void Movement()
    {
        if (m_CantMove)
            return;

        targetDir.Normalize();
        m_CurrentDir = Vector2.SmoothDamp(m_CurrentDir, targetDir, ref currentDirVelocity, m_MoveSmoothTime);

        if (m_Controller.isGrounded && !isJumping)
            velocityY = 0;

        velocityY += m_Gravity * Time.deltaTime;

        velocity = (transform.forward * m_CurrentDir.y + transform.right * m_CurrentDir.x) * speed + Vector3.up * velocityY;

        m_Controller.Move(velocity * Time.deltaTime);

        if ((Mathf.Abs(targetDir.x) > 0 || Mathf.Abs(targetDir.y) > 0) && onSlope())
            m_Controller.Move(Vector3.down * m_Controller.height / 2 * slopeForce * Time.deltaTime);

        if (m_Controller.isGrounded)
        {
            if (m_WalkingSpeed != w || m_RunningSpeed != r)
                SetSpeed();
        }

        if ((m_Controller.collisionFlags & CollisionFlags.Below) != 0)
        {
        }
    }

    void Running()
    {
        if (m_Controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (speed < m_RunningSpeed)
                    speed = m_RunningSpeed;
            }
            else
            {
                if (speed > m_WalkingSpeed)
                    speed = m_WalkingSpeed;
            }
        }
    }

    bool onSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_Controller.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;

        return false;
    }

    void Jump()
    {
        isJumping = true;
        velocityY = m_JumpForce;

        StartCoroutine(BackToGround());
    }

    public void FreezePlayer(bool active)
    {
        m_CantLook = active;
        m_CantMove = active;
        m_CantJump = active;
        m_CurrentDir = Vector2.zero;
    }

    public void FreezePlayerCamera(bool active)
    {
        m_CantLook = active;
    }

    public void PausePlayer(bool active)
    {
        m_CantLook = active;
        m_CantMove = active;
        m_CurrentDir = Vector2.zero;

        if(active)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private IEnumerator BackToGround()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.1f);

        m_WalkingSpeed = 5;
        m_RunningSpeed = 7;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = m_RunningSpeed;

        yield return wfs;
        isJumping = false;
    }
 
    public void SetSpeed()
    {
        m_WalkingSpeed = w;
        m_RunningSpeed = r;
        speed = w;
    }
    
    public void SetCameraState(bool state)
    {
        m_CameraFps.enabled = state;
    }

}
