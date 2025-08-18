using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ссылки")]
    [HideInInspector] public CharacterController controller;
    private KeyRebinder keyRebinder;
    private AdminPanel adminPanel;
    private Animator animator;
    private Score_Timer scoreTimer;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float sneakSpeed = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    public bool MovementLock;
    public float currentSpeed;
    private Vector3 movementDirection;
    private Vector3 velocity;

    [Header("Camera Settings")]
    public Camera playerCamera;
    [SerializeField] private float minVerticalAngle = -80f; // Макс. угол вниз
    [SerializeField] private float maxVerticalAngle = 80f;  // Макс. угол вверх
    [SerializeField] private float rotationSpeed = 2f;

    [SerializeField] private bool _invertingX;
    [SerializeField] private bool _invertingY;

    public bool CameraLock = false;
    public bool CameraLockX = false;
    public bool CameraLockY = false;
    public float currentCameraRotationX = -180f; // Текущий угол камеры по X

    [Header ("Bools")]
    private bool isGrounded;
    private bool isSprinting;
    private bool isSneaking;

    [HideInInspector] public bool isTime_TaskOn;
    [HideInInspector] public bool isTime_TaskOff;
    [HideInInspector] public bool isTime_Task = false;

    private bool isTelekinesisOn;
    private bool isTelekinesisOff;
    private bool isTelekinesis = false;

    [Header ("Health")]
    public int MaxPlayerHealth = 4;
    public int PlayerHealth;

    [Header ("Checks")]
    private bool isAdminPanel;

    [SerializeField] private TextMeshPro _playerHealth;

    [SerializeField] private GameObject telekinesisTool;

    private void Awake()
    {

        #region Links
        adminPanel = FindAnyObjectByType<AdminPanel>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        scoreTimer = FindAnyObjectByType<Score_Timer>();
        #endregion

        #region Checks
        if (adminPanel == null)
        {
            isAdminPanel = false;
        }

        else
        {
            isAdminPanel = true;
        }
        #endregion

        currentSpeed = runSpeed;
        PlayerHealth = MaxPlayerHealth;
    }

    private void Start()
    {
        #region Player Settings
        runSpeed = PlayerManager.Instance.PlayerStats.playerRunSpeed;
        sprintSpeed = PlayerManager.Instance.PlayerStats.playerSprintSpeed;
        sneakSpeed = PlayerManager.Instance.PlayerStats.playerSneekSpeed;
        jumpHeight = PlayerManager.Instance.PlayerStats.playerJumpHeight;

        gravity = PlayerManager.Instance.PlayerStats.gravity;

        rotationSpeed = PlayerManager.Instance.PlayerStats.rotationSpeed;
        _invertingX = PlayerManager.Instance.PlayerStats.invertingX;
        _invertingY = PlayerManager.Instance.PlayerStats.invertingY;

        MaxPlayerHealth = DifficultyManager.Instance.CurrentDifficulty.playerHealth;
        #endregion
    }
    private void Update()
    {
        if (adminPanel.InfinityHealthOn)
        {
            _playerHealth.text = "Здоровье: " + "\u221E";
        }

        else if (!adminPanel.InfinityHealthOn)
        {
            _playerHealth.text = null;
            _playerHealth.text = "Здоровье: " + PlayerHealth.ToString();
        }

        if (transform.position.y <= -15) { transform.position = new Vector3(2, 0, 0); scoreTimer.TimeLeft = 0; }

        handleInput();
        handleCameraRotation();
        updateAnimations();
    }

    private void FixedUpdate()
    {
        handleMovement();
        handleGravity();
    }

    public void openTelekinesis()
    {
        AchievementManager.Instance.UnlockAchievement("TakeTelekinesis");
    }

    private void handleInput()
    {
        isGrounded = controller.isGrounded;

        // Сброс вертикальной скорости при нахождении на земле
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.ResetTrigger("Jump");
        }

        // Обработка спринта
        isSprinting = keyRebinder.GetAction("Sprint");
        isSneaking = keyRebinder.GetAction("Sneek");

        if (keyRebinder.GetActionDown("Time&Task"))
        {
            if (!isTime_Task)
            {
                isTime_Task = true;
                isTime_TaskOn = true;
                isTime_TaskOff = false;
            }
            else if (isTime_Task)
            {
                isTime_Task = false;
                isTime_TaskOn = false;
                isTime_TaskOff = true;
            }
        }

        if (AchievementManager.Instance.IsAchievementUnlocked("TakeTelekinesis"))
        {
            telekinesisTool.SetActive(true);

            if (keyRebinder.GetActionDown("Telekinesis"))
            {
                if (!isTelekinesis)
                {
                    isTelekinesis = true;
                    isTelekinesisOn = true;
                    isTelekinesisOff = false;
                }

                else if (isTelekinesis)
                {
                    isTelekinesis = false;
                    isTelekinesisOn = false;
                    isTelekinesisOff = true;
                }
            }
        }
    }

    private void handleMovement()
    {
        if (MovementLock == true)
        {
            isSprinting = false;
            movementDirection = Vector3.zero;
            animator.ResetTrigger("Jump");
            return;
        }

        movementDirection = Vector3.zero;
        // Движение относительно локальных осей персонажа
        if (keyRebinder.GetAction("Movement Forward")) movementDirection += transform.forward;
        if (keyRebinder.GetAction("Movement Back")) movementDirection -= transform.forward;
        if (keyRebinder.GetAction("Movement Right")) movementDirection += transform.right;
        if (keyRebinder.GetAction("Movement Left")) movementDirection -= transform.right;

        // Нормализация и применение скорости
        if (movementDirection != Vector3.zero)
        {
            movementDirection.Normalize();
            currentSpeed = isSprinting ? sprintSpeed: isSneaking ? sneakSpeed: runSpeed;
            controller.Move(movementDirection * currentSpeed * Time.fixedDeltaTime);
        }

        // От проваливания в лифте
        if (isGrounded) controller.Move(Vector3.down * 1 * Time.fixedDeltaTime);

        // Прыжок
        if (keyRebinder.GetActionDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    private void handleGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }

    private void handleCameraRotation()
    {
        if (CameraLock || CameraLockX || CameraLockY) return;

        // Получаем ввод мыши
        float mouseX = CameraLockX ? 0 : Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = CameraLockY ? 0 : Input.GetAxis("Mouse Y") * rotationSpeed;

        if (_invertingX == true)
        {
            mouseX = -mouseX;
        }

        if (_invertingY == true)
        {
            mouseY = -mouseY;
        }

        // Вращение персонажа по горизонтали
        transform.Rotate(Vector3.up, mouseX);

        // Вращение камеры по вертикали с ограничением
        currentCameraRotationX -= mouseY;
        currentCameraRotationX = Mathf.Clamp(
            currentCameraRotationX,
            minVerticalAngle,
            maxVerticalAngle
        );

        // Применяем поворот камеры
        playerCamera.transform.localEulerAngles = new Vector3(
            currentCameraRotationX,
            0f,
            0f
        );
    }

    private void updateAnimations()
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
        animator.SetBool("IsTelekinesisOn", isTelekinesisOn);
        animator.SetBool("IsTelekinesisOff", isTelekinesisOff);
    }

}