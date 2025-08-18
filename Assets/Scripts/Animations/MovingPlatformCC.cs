using UnityEngine;

public class MovingPlatformCC : MonoBehaviour
{
    private Vector3 _lastPosition; // Позиция платформы в прошлом кадре
    private Vector3 _currentVelocity; // Текущая скорость платформы

    void Start()
    {
        _lastPosition = transform.position;
    }

    void LateUpdate()
    {
        // Вычисляем скорость платформы (для передачи персонажу)
        _currentVelocity = (transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;
    }

    // Если персонаж стоит на платформе, двигаем его вручную
    public Vector3 GetPlatformVelocity()
    {
        return _currentVelocity;
    }
}