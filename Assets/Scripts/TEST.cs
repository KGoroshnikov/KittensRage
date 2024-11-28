using UnityEngine;

public class TEST : MonoBehaviour
{
    [Header("Настройки катапульты")]
    public GameObject projectilePrefab;      // Префаб снаряда
    public Transform slingOrigin;            // Точка старта снаряда
    public float maxStretch = 3f;            // Максимальная длина натяжения
    public float launchForceMultiplier = 10f; // Множитель силы запуска
    public float maxAngle = 60f;
    public float minAngle = 0f;

    [Header("Траектория")]
    public LineRenderer trajectoryLine;      // Линия для отображения траектории
    public int trajectoryPointsCount = 30;   // Количество точек траектории

    // Для нестандартной работы гравитации
    public bool isNonStandardGravityEnabled = false; // Режим нестандартной гравитации
    public float gravityStrength = 9.81f;  // Сила гравитации для нестандартного режима
    

    private GameObject currentProjectile;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 startPoint;

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (currentProjectile != null)
        {
            if (isNonStandardGravityEnabled && !rb.isKinematic)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                // Применение нестандартной гравитации по оси Z
                rb.AddForce(Vector3.back * gravityStrength, ForceMode.Acceleration);  // Гравитация по оси Z
            }
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Dragging();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            ReleaseProjectile();
        }
    }

    private void StartDragging()
    {
        currentProjectile = Instantiate(projectilePrefab, slingOrigin.position, Quaternion.identity);
        rb = currentProjectile.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        startPoint = GetMouseWorldPosition();
        if (!isNonStandardGravityEnabled)
            startPoint.z = slingOrigin.position.z;  // Сохраняем z-координату на старте
        isDragging = true;
    }

    private void Dragging()
    {
        Vector3 currentPoint = GetMouseWorldPosition();
        if (!isNonStandardGravityEnabled)
            currentPoint.z = slingOrigin.position.z; // Сохраняем z-координату во время перетаскивания

        Vector3 direction = startPoint - currentPoint;

        // В расчете угла нужно учитывать ось Z для нестандартной гравитации
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        if (!isNonStandardGravityEnabled)angle = Mathf.Clamp(angle, minAngle, maxAngle);
        Debug.Log("Angle: " + angle);

        float clampedMagnitude = Mathf.Min(direction.magnitude, maxStretch);

        // Преобразуем направляющий вектор для учета оси Z при нестандартной гравитации
        if (!isNonStandardGravityEnabled)
            direction = new Vector3(
                clampedMagnitude * Mathf.Cos(angle * Mathf.Deg2Rad),
                clampedMagnitude * Mathf.Sin(angle * Mathf.Deg2Rad),
                0);  // Для стрельбы по X-Y

        currentProjectile.transform.position = slingOrigin.position - direction;
        DrawTrajectory(direction);
    }

    private void ReleaseProjectile()
    {
        isDragging = false;

        Vector3 launchDirection = slingOrigin.position - currentProjectile.transform.position;
        if (!isNonStandardGravityEnabled)
            launchDirection.z = 0;  // Для запуска только по осям X и Y

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(launchDirection * launchForceMultiplier, ForceMode.Impulse);

        trajectoryLine.positionCount = 0;
    }

    private void DrawTrajectory(Vector3 launchDirection)
    {
        Vector3 startPosition = currentProjectile.transform.position;
        Vector3 velocity = launchDirection * launchForceMultiplier / currentProjectile.GetComponent<Rigidbody>().mass;

        trajectoryLine.positionCount = trajectoryPointsCount;
        for (int i = 0; i < trajectoryPointsCount; i++)
        {
            float time = i * 0.1f;
            Vector3 displacement = Vector3.zero;
            if (!isNonStandardGravityEnabled) 
                displacement.z = 0;  // Если стандартная гравитация, то она действует только по Y

            // Если нестандартная гравитация, то траектория учитывает ось Z
            if (isNonStandardGravityEnabled)
            {
                displacement = velocity * time + 0.5f * Vector3.back * gravityStrength * time * time;
            }
            else{
                displacement = velocity * time + 0.5f * Physics.gravity * time * time;
            }

            Vector3 drawPoint = startPosition + displacement;
            if (!isNonStandardGravityEnabled) drawPoint.z = slingOrigin.position.z;  // Фиксируем Z-координату для визуализации
            trajectoryLine.SetPosition(i, drawPoint);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(slingOrigin.position).z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (!isNonStandardGravityEnabled) worldPosition.z = slingOrigin.position.z;  // Фиксируем Z-координату
        return worldPosition;
    }
}
