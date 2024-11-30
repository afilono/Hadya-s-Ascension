using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Ссылка на объект игрока
    public float smoothSpeed = 0.125f; // Скорость сглаживания
    public Vector3 offset; // Смещение камеры относительно игрока

    void LateUpdate()
    {
        // Желаемая позиция камеры
        Vector3 desiredPosition = player.position + offset;

        // Сглаженное движение камеры
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Обновляем позицию камеры
        transform.position = smoothedPosition;
    }
}
