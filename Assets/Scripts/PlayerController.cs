using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private KeyRebinder keyRebinder;
    private CharacterController controller;
    private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Camera Settings")]
    public Camera playerCamera;
    [SerializeField] private float minVerticalAngle = -80f; // ����. ���� ����
    [SerializeField] private float maxVerticalAngle = 80f;  // ����. ���� �����
    [SerializeField] private float rotationSpeed = 2f;
    private float currentCameraRotationX = -180f; // ������� ���� ������ �� X



    private Vector3 movementDirection;
    private Vector3 velocity;
    private float currentSpeed;
    private bool isGrounded;
    private bool isSprinting;

    [Header("Pause Settings")]
    [SerializeField] private Pause pauseManager; // ������ �� ������ �����

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        keyRebinder = FindObjectOfType<KeyRebinder>();
        pauseManager = FindObjectOfType<Pause>(); // ������� ������ �����, ���� �� ����� �������

        currentSpeed = walkSpeed;
    }


    private void Update()
    {
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
    }

    private void HandleMovement()
    {
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
            currentSpeed = isSprinting ? runSpeed : walkSpeed;
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
        if (pauseManager != null && pauseManager.IsActive)
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
        bool isActuallySprinting = isMoving && isSprinting;

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSprinting", isActuallySprinting);
        animator.SetBool("IsGrounded", isGrounded);
    }


}