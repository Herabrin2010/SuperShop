using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 CameraMovement;
    public Camera Camera;

    private float Horizontal;
    private float Vertical;

    private float currentSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] public float gravity;
    [SerializeField] public float jumpHeight;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private Animator animator;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        InputKey();
        Rotation();
    }

    private void FixedUpdate()
    {
        Vector3 global = new Vector3(Horizontal, 0, Vertical);

        Vector3 local = transform.TransformDirection(global);


        transform.Rotate(Vector3.up, CameraMovement.x*rotationSpeed);

        if(Camera.transform.rotation.x < 90 & Camera.transform.rotation.x > -90)
            Camera.transform.Rotate(-Vector3.right, CameraMovement.y * rotationSpeed);
    }

    private void InputKey()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Фиксим баг с "залипанием" в земле
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Run");
        }

        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Гравитация
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
            animator.SetTrigger("Sprint");
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.ResetTrigger("Sprint");
            animator.SetTrigger("Run");
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.SetTrigger("Idle");
            animator.ResetTrigger("Run");
            animator.ResetTrigger("Sprint");
        }

    }

    private void Rotation()
    {
        CameraMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
