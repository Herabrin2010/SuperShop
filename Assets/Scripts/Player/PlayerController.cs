using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("������")]
    private KeyRebinder keyRebinder;
    private CharacterController controller;
    private AdminPanel adminPanel;
    private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float sneakSpeed = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [HideInInspector] public bool MovementLock;
    [HideInInspector] public float currentSpeed;
    private Vector3 movementDirection;
    private Vector3 velocity;

    [Header("Camera Settings")]
    public Camera playerCamera;
    [SerializeField] private float minVerticalAngle = -80f; // ����. ���� ����
    [SerializeField] private float maxVerticalAngle = 80f;  // ����. ���� �����
    [SerializeField] private float rotationSpeed = 2f;
    [HideInInspector] public bool CameraLock;
    [HideInInspector] public float currentCameraRotationX = -180f; // ������� ���� ������ �� X

    [Header ("Bools")]
    private bool isGrounded;
    private bool isSprinting;
    private bool isSneaking;

    [HideInInspector] public bool isTime_TaskOn;
    [HideInInspector] public bool isTime_TaskOff;
    [HideInInspector] public bool isTime_Task = false;

    public int MaxPlayerHealth = 4;
    public int PlayerHealth;

    [SerializeField] private TextMeshPro _playerHealth;

    private void Start()
    {
        adminPanel = FindAnyObjectByType<AdminPanel>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        keyRebinder = FindAnyObjectByType<KeyRebinder>();

        currentSpeed = walkSpeed;
        PlayerHealth = MaxPlayerHealth;
    }


    private void Update()
    {
        if (adminPanel.InfinityHealthOn == true)
        {
            _playerHealth.text = "��������: " + "\u221E"; 
        }

        else
        {
            _playerHealth.text = null;
            _playerHealth.text = "��������: " + PlayerHealth.ToString(); 
        }

        HandleInput();
        HandleCameraRotation();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
    }
    private void HandleInput()
    {
        isGrounded = controller.isGrounded;

        // ����� ������������ �������� ��� ���������� �� �����
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.ResetTrigger("Jump");
        }

        // ��������� �������
        isSprinting = keyRebinder.GetAction("Sprint");
        isSneaking = keyRebinder.GetAction("Sneek");

        if (keyRebinder.GetActionDown("Time&Task"))
        {
            if (isTime_Task == false)
            {
                isTime_Task = true;
                isTime_TaskOn = true;
                isTime_TaskOff = false;
            }
            else if (isTime_Task == true)
            {
                isTime_Task = false;
                isTime_TaskOn = false;
                isTime_TaskOff = true;
            }
        }
    }

    private void HandleMovement()
    {
        if (MovementLock == true)
        {
            isSprinting = false;
            movementDirection = Vector3.zero;
            animator.ResetTrigger("Jump");
            return;
        }

        movementDirection = Vector3.zero;
        // �������� ������������ ��������� ���� ���������
        if (keyRebinder.GetAction("Movement Forward")) movementDirection += transform.forward;
        if (keyRebinder.GetAction("Movement Back")) movementDirection -= transform.forward;
        if (keyRebinder.GetAction("Movement Right")) movementDirection += transform.right;
        if (keyRebinder.GetAction("Movement Left")) movementDirection -= transform.right;

        // ������������ � ���������� ��������
        if (movementDirection != Vector3.zero)
        {
            movementDirection.Normalize();
            currentSpeed = isSprinting ? runSpeed: isSneaking ? sneakSpeed: walkSpeed;
            controller.Move(movementDirection * currentSpeed * Time.fixedDeltaTime);
        }

        // ������
        if (keyRebinder.GetActionDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    private void HandleGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }

    private void HandleCameraRotation()
    {
        // ���� ���� ����� ������� - �� ������� ������
        if (CameraLock == true)
            return;

        // �������� ���� ����
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // �������� ��������� �� �����������
        transform.Rotate(Vector3.up, mouseX);

        // �������� ������ �� ��������� � ������������
        currentCameraRotationX -= mouseY;
        currentCameraRotationX = Mathf.Clamp(
            currentCameraRotationX,
            minVerticalAngle,
            maxVerticalAngle
        );

        // ��������� ������� ������
        playerCamera.transform.localEulerAngles = new Vector3(
            currentCameraRotationX,
            0f,
            0f
        );
    }

    private void UpdateAnimations()
    {
        bool isMoving = movementDirection != Vector3.zero;
        bool isActuallySneaking = isMoving && isSneaking;
        bool isActuallySprinting = isMoving && isSprinting;

        animator.SetBool("IsSneaking", isActuallySneaking);
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSprinting", isActuallySprinting);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsTime&TaskOn", isTime_TaskOn);
        animator.SetBool("IsTime&TaskOff", isTime_TaskOff);
    }

}