using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f; // Базовая скорость движения
    [SerializeField] private float zoomSpeed = 0.5f; // Скорость масштабирования
    [SerializeField] private float minZoom = 5f; // Минимальный зум
    [SerializeField] private float maxZoom = 50f; // Максимальный зум
    [SerializeField] private float smoothTime = 0.1f; // Время сглаживания движения

    private Vector2 lastTouchPosition; // Последняя позиция касания
    private Vector3 velocity = Vector3.zero; // Текущая скорость движения камеры
    private Vector3 targetPosition; // Целевая позиция камеры
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        targetPosition = cam.transform.position; // Устанавливаем начальную цель
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                velocity = Vector3.zero; // Сбрасываем инерцию
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // Рассчитываем направление движения
                Vector2 delta = (Vector2)touch.position - lastTouchPosition;
                lastTouchPosition = touch.position;

                // Игнорируем наклон по X, используя проекцию направления на горизонтальную плоскость
                Vector3 forward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 right = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

                // Преобразуем движение в зависимости от горизонтального направления камеры
                Vector3 direction = right * -delta.x + forward * -delta.y;

                // Обновляем целевую позицию
                targetPosition += direction * moveSpeed;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomSpeed);

            // Сбрасываем цель, чтобы избежать конфликта между зумом и движением
            targetPosition = cam.transform.position;
        }

        // Плавно движем камеру к целевой позиции
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, smoothTime);
    }

    void Zoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, minZoom, maxZoom);
    }
}
