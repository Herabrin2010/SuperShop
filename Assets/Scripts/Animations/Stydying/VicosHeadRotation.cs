using UnityEngine;

public class VicosHeadRotation : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5f;

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;  // Игнорируем вертикальную разницу

        // Поворот только вокруг оси Y
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float targetYRotation = targetRotation.eulerAngles.y;

        // Текущий поворот
        float currentYRotation = transform.eulerAngles.y;

        // Плавное изменение угла
        float newYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, rotationSpeed * Time.deltaTime);

        // Применяем поворот только по Y
        transform.eulerAngles = new Vector3(0, newYRotation, 0);
    }
}
