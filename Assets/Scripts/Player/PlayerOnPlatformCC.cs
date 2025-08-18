using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerOnPlatformCC : MonoBehaviour
{
    private CharacterController _controller;
    private GameObject _currentPlatform;
    private float _groundCheckDistance = 0.1f; // Дистанция проверки земли
    private float _stickToGroundForce = 0.3f; // Сила "прилипания" к полу

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckGround();
        MoveWithPlatform();
    }

    void CheckGround()
    {
        // Если персонаж в воздухе, но по физике должен быть на земле — "прижимаем" его вниз
        if (!_controller.isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, _groundCheckDistance))
            {
                // Принудительно двигаем вниз, чтобы "прилипнуть"
                _controller.Move(Vector3.down * _stickToGroundForce * Time.deltaTime);
            }
        }
    }

    void MoveWithPlatform()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 0.2f))
        {
            if (hit.collider.CompareTag("MovingPlatform"))
            {
                var platformVelocity = hit.collider.GetComponent<MovingPlatformCC>().GetPlatformVelocity();
                _controller.Move(platformVelocity * Time.deltaTime);
            }
        }
    }
}